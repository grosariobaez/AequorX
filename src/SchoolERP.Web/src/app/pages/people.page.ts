import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CoreDomainApiService } from '../core/core-domain-api.service';
import { Person } from '../core/core-domain.models';
import { I18nService } from '../core/i18n.service';

@Component({
  imports: [FormsModule],
  templateUrl: './people.page.html',
})
export class PeoplePage {
  protected readonly i18n = inject(I18nService);
  private readonly api = inject(CoreDomainApiService);

  protected readonly people = signal<Person[]>([]);
  protected readonly error = signal<string | null>(null);
  protected search = '';
  protected form = { firstName: '', lastName: '', email: '', phone: '' };

  constructor() {
    this.load();
  }

  protected load(): void {
    this.error.set(null);
    this.api.get<Person[]>('/api/people', this.search).subscribe({
      next: (people) => this.people.set(people),
      error: () => this.error.set(this.i18n.text('loadError')),
    });
  }

  protected create(): void {
    this.error.set(null);
    this.api.post<Person>('/api/people', this.form).subscribe({
      next: () => {
        this.form = { firstName: '', lastName: '', email: '', phone: '' };
        this.load();
      },
      error: () => this.error.set(this.i18n.text('saveError')),
    });
  }
}
