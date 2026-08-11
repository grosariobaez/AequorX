import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { AcademicYear, Campus, GradeLevel, Identifier, Section } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './sections.page.html',
})
export class SectionsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly sections = signal<Section[]>([]);
  protected readonly years = signal<AcademicYear[]>([]);
  protected readonly grades = signal<GradeLevel[]>([]);
  protected readonly campuses = signal<Campus[]>([]);
  protected readonly error = signal<string | null>(null);
  protected campusForm = { name: '', code: '' };
  protected form = {
    academicYearId: '',
    gradeLevelId: '',
    campusId: '',
    name: '',
    code: '',
    capacity: null as number | null,
  };

  constructor() {
    this.load();
  }

  protected load(): void {
    forkJoin({
      sections: this.api.get<Section[]>('/api/sections'),
      years: this.api.get<AcademicYear[]>('/api/academic-years'),
      grades: this.api.get<GradeLevel[]>('/api/grade-levels'),
      campuses: this.api.get<Campus[]>('/api/campuses'),
    }).subscribe({
      next: ({ sections, years, grades, campuses }) => {
        this.sections.set(sections);
        this.years.set(years);
        this.grades.set(grades);
        this.campuses.set(campuses);
      },
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected createCampus(): void {
    this.api.post<Identifier>('/api/campuses', this.campusForm).subscribe({
      next: () => {
        this.campusForm = { name: '', code: '' };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }

  protected create(): void {
    this.api.post<Identifier>('/api/sections', this.form).subscribe({
      next: () => {
        this.form = { academicYearId: '', gradeLevelId: '', campusId: '', name: '', code: '', capacity: null };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
