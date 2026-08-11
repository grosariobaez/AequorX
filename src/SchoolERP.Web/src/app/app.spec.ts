import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Home } from './home';
import { routes } from './app.routes';

describe('Home', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('renders the bootstrap shell and reports a healthy API', () => {
    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();

    const request = TestBed.inject(HttpTestingController).expectOne('/health/live');
    request.flush('Healthy');
    fixture.detectChanges();

    const page = fixture.nativeElement as HTMLElement;
    expect(page.querySelector('h1')?.textContent).toContain('Fundación del sistema');
    expect(page.querySelector('.healthy')?.textContent).toContain('Disponible');
  });

  it('is the explicit root route', () => {
    expect(routes[0]).toEqual({ path: '', component: Home, pathMatch: 'full' });
  });
});
