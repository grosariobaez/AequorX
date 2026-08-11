import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { Assessment, GradeRoster, GradeStudent, Section } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

interface GradeEdit extends GradeStudent {
  originalScore: number | null;
  correctionScore: number | null;
  correctionReason: string;
}

@Component({ imports: [FormsModule], templateUrl: './grades.page.html' })
export class GradesPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);
  protected readonly sections = signal<Section[]>([]);
  protected readonly assessments = signal<Assessment[]>([]);
  protected readonly grades = signal<GradeEdit[]>([]);
  protected readonly maximumScore = signal(0);
  protected readonly error = signal<string | null>(null);
  protected sectionId = '';
  protected assessmentId = '';
  protected assessmentForm = { name: '', assessmentDate: '', maximumScore: 100 };

  constructor() {
    this.api.get<Section[]>('/api/sections').subscribe({
      next: (value) => this.sections.set(value),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected loadAssessments(): void {
    if (!this.sectionId) return;
    this.api.getWithParams<Assessment[]>('/api/assessments', { sectionId: this.sectionId }).subscribe({
      next: (value) => { this.assessments.set(value); this.grades.set([]); },
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected createAssessment(): void {
    this.api.post<Assessment>('/api/assessments', { sectionId: this.sectionId, ...this.assessmentForm }).subscribe({
      next: () => { this.assessmentForm = { name: '', assessmentDate: '', maximumScore: 100 }; this.loadAssessments(); },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }

  protected loadGrades(): void {
    if (!this.assessmentId) return;
    this.api.get<GradeRoster>(`/api/assessments/${this.assessmentId}/grades`).subscribe({
      next: (roster) => {
        this.maximumScore.set(roster.maximumScore);
        this.grades.set(roster.students.map((grade) => ({ ...grade, originalScore: grade.score, correctionScore: null, correctionReason: '' })));
      },
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected saveDrafts(): void {
    const changed = this.grades().filter((grade) => grade.score !== null && grade.score !== grade.originalScore && !grade.status?.match(/Published|Corrected/));
    if (!changed.length) return;
    forkJoin(changed.map((grade) => this.api.put<GradeStudent>(
      `/api/assessments/${this.assessmentId}/grades/${grade.enrollmentId}`, { score: grade.score }))).subscribe({
      next: () => this.loadGrades(), error: () => this.error.set(this.i18n.text('saveError')),
    });
  }

  protected publish(): void {
    this.api.post(`/api/assessments/${this.assessmentId}/publish`, {}).subscribe({
      next: () => this.loadGrades(), error: () => this.error.set(this.i18n.text('saveError')),
    });
  }

  protected correct(grade: GradeEdit): void {
    if (!grade.gradeId || grade.correctionScore === null || !grade.correctionReason.trim()) return;
    this.api.post(`/api/grades/${grade.gradeId}/corrections`, {
      score: grade.correctionScore, reason: grade.correctionReason,
    }).subscribe({ next: () => this.loadGrades(), error: () => this.error.set(this.i18n.text('saveError')) });
  }
}
