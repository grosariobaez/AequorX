import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ClassesPage } from './classes.page';

describe('Subject and class workflow', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [ClassesPage], providers: [provideHttpClient(), provideHttpClientTesting()] }).compileComponents();
  });
  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('adds a subject as a class in the selected section', async () => {
    const fixture = TestBed.createComponent(ClassesPage);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne('/api/sections').flush([{ id: 'section-1', name: 'A', code: 'A', capacity: 30, academicYearId: 'year-1', academicYearName: '2026-2027', gradeLevelId: 'grade-1', gradeLevelName: 'Primero', campusId: 'campus-1', campusName: 'Principal' }]);
    http.expectOne('/api/subjects').flush([{ id: 'subject-1', name: 'Matemáticas', code: 'MAT', isActive: true }]);
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const section = fixture.nativeElement.querySelector('select') as HTMLSelectElement;
    section.value = 'section-1'; section.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges();
    (fixture.nativeElement.querySelector('form button') as HTMLButtonElement).click();
    http.expectOne((request) => request.url === '/api/classes' && request.params.get('sectionId') === 'section-1').flush([]);
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const subject = fixture.nativeElement.querySelectorAll('select')[1] as HTMLSelectElement;
    subject.value = 'subject-1'; subject.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges();
    (fixture.nativeElement.querySelectorAll('form')[1].querySelector('button') as HTMLButtonElement).click();
    const create = http.expectOne('/api/classes');
    expect(create.request.method).toBe('POST');
    expect(create.request.body).toEqual({ sectionId: 'section-1', subjectId: 'subject-1' });
    create.flush({ id: 'class-1', sectionId: 'section-1', subjectId: 'subject-1', name: 'Matemáticas · A', subjectName: 'Matemáticas', subjectCode: 'MAT', isActive: true });
    http.expectOne((request) => request.url === '/api/classes' && request.params.get('sectionId') === 'section-1')
      .flush([{ id: 'class-1', sectionId: 'section-1', subjectId: 'subject-1', name: 'Matemáticas · A', subjectName: 'Matemáticas', subjectCode: 'MAT', isActive: true }]);
  });
});
