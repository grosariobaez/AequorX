import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  imports: [RouterLink],
  template: `
    <section aria-labelledby="not-found-title">
      <p class="eyebrow">404</p>
      <h2 id="not-found-title">Página no encontrada</h2>
      <p>La ruta solicitada no existe en esta base técnica.</p>
      <a routerLink="/">Volver al inicio</a>
    </section>
  `,
})
export class NotFound {}
