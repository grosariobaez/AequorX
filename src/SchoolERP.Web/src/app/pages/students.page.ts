import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { Person, Student } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './students.page.html',
})
export class StudentsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly students = signal<Student[]>([]);
  protected readonly people = signal<Person[]>([]);
  protected readonly error = signal<string | null>(null);
  protected form = { personId: '', studentNumber: '' };

  constructor() {
    this.load();
  }

  protected load(): void {
    forkJoin({
      students: this.api.get<Student[]>('/api/students'),
      people: this.api.get<Person[]>('/api/people'),
    }).subscribe({
      next: ({ students, people }) => {
        this.students.set(students);
        this.people.set(people);
      },
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    this.api.post<Student>(
      `/api/students/${this.form.personId}/profile`,
      { studentNumber: this.form.studentNumber },
    ).subscribe({
      next: () => {
        this.form = { personId: '', studentNumber: '' };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
