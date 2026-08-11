import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { AcademicYear, Enrollment, Identifier, Section, Student } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './enrollments.page.html',
})
export class EnrollmentsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly enrollments = signal<Enrollment[]>([]);
  protected readonly students = signal<Student[]>([]);
  protected readonly years = signal<AcademicYear[]>([]);
  protected readonly sections = signal<Section[]>([]);
  protected readonly error = signal<string | null>(null);
  protected form = {
    studentPersonId: '',
    academicYearId: '',
    sectionId: '',
    status: 'Pending',
    enrollmentDate: '',
  };

  constructor() {
    this.load();
  }

  protected load(): void {
    forkJoin({
      enrollments: this.api.get<Enrollment[]>('/api/enrollments'),
      students: this.api.get<Student[]>('/api/students'),
      years: this.api.get<AcademicYear[]>('/api/academic-years'),
      sections: this.api.get<Section[]>('/api/sections'),
    }).subscribe({
      next: ({ enrollments, students, years, sections }) => {
        this.enrollments.set(enrollments);
        this.students.set(students);
        this.years.set(years);
        this.sections.set(sections);
      },
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    this.api.post<Identifier>('/api/enrollments', this.form).subscribe({
      next: () => {
        this.form = { studentPersonId: '', academicYearId: '', sectionId: '', status: 'Pending', enrollmentDate: '' };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
