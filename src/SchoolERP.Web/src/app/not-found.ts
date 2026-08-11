import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { I18nService } from './core/i18n.service';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  template: `
    <section aria-labelledby="not-found-title">
      <p class="eyebrow">404</p>
      <h2 id="not-found-title">{{ i18n.text('notFound') }}</h2>
      <p>{{ i18n.text('routeMissing') }}</p>
      <a routerLink="/">{{ i18n.text('backHome') }}</a>
    </section>
  `,
})
export class NotFound {
  protected readonly i18n = inject(I18nService);
}
