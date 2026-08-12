import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { SchoolClass, Section, Subject } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({ imports: [FormsModule], templateUrl: './classes.page.html' })
export class ClassesPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);
  protected readonly sections = signal<Section[]>([]);
  protected readonly subjects = signal<Subject[]>([]);
  protected readonly classes = signal<SchoolClass[]>([]);
  protected readonly error = signal<string | null>(null);
  protected sectionId = '';
  protected subjectId = '';

  constructor() {
    forkJoin({ sections: this.api.get<Section[]>('/api/sections'), subjects: this.api.get<Subject[]>('/api/subjects') })
      .subscribe({ next: ({ sections, subjects }) => { this.sections.set(sections); this.subjects.set(subjects); }, error: () => this.error.set(this.i18n.text('loadError')) });
  }

  protected load(): void {
    if (!this.sectionId) return;
    this.api.getWithParams<SchoolClass[]>('/api/classes', { sectionId: this.sectionId }).subscribe({
      next: (value) => this.classes.set(value), error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    if (!this.sectionId || !this.subjectId) return;
    this.api.post<SchoolClass>('/api/classes', { sectionId: this.sectionId, subjectId: this.subjectId }).subscribe({
      next: () => { this.subjectId = ''; this.load(); }, error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
