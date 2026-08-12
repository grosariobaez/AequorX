import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { Subject } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({ imports: [FormsModule], templateUrl: './subjects.page.html' })
export class SubjectsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);
  protected readonly subjects = signal<Subject[]>([]);
  protected readonly error = signal<string | null>(null);
  protected form = { name: '', code: '' };

  constructor() { this.load(); }

  protected create(): void {
    this.api.post<Subject>('/api/subjects', this.form).subscribe({
      next: () => { this.form = { name: '', code: '' }; this.load(); },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }

  private load(): void {
    this.api.get<Subject[]>('/api/subjects').subscribe({
      next: (value) => this.subjects.set(value),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }
}
