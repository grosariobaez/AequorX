import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { GradeLevel } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './grade-levels.page.html',
})
export class GradeLevelsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly grades = signal<GradeLevel[]>([]);
  protected readonly error = signal<string | null>(null);
  protected form = { name: '', code: '', sortOrder: 1 };

  constructor() {
    this.load();
  }

  protected load(): void {
    this.api.get<GradeLevel[]>('/api/grade-levels').subscribe({
      next: (grades) => this.grades.set(grades),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    this.api.post<GradeLevel>('/api/grade-levels', this.form).subscribe({
      next: () => {
        this.form = { name: '', code: '', sortOrder: 1 };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
