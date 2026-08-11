import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('renders the bootstrap shell and reports a healthy API', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const request = TestBed.inject(HttpTestingController).expectOne('/health/live');
    request.flush('Healthy');
    fixture.detectChanges();

    const page = fixture.nativeElement as HTMLElement;
    expect(page.querySelector('h1')?.textContent).toContain('Fundación del sistema');
    expect(page.querySelector('.healthy')?.textContent).toContain('Disponible');
  });
});
