import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { AcademicYear } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './academic-years.page.html',
})
export class AcademicYearsPage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly years = signal<AcademicYear[]>([]);
  protected readonly error = signal<string | null>(null);
  protected form = { name: '', startDate: '', endDate: '', status: 'Planned' };

  constructor() {
    this.load();
  }

  protected load(): void {
    this.api.get<AcademicYear[]>('/api/academic-years').subscribe({
      next: (years) => this.years.set(years),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    this.api.post<AcademicYear>('/api/academic-years', this.form).subscribe({
      next: () => {
        this.form = { name: '', startDate: '', endDate: '', status: 'Planned' };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
