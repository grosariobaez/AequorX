import { Injectable, signal } from '@angular/core';

export type Language = 'es' | 'en';

const translations = {
  es: {
    appSubtitle: 'Administración escolar',
    language: 'Idioma',
    spanish: 'Español',
    english: 'English',
    people: 'Personas',
    students: 'Estudiantes',
    academicStructure: 'Estructura académica',
    academicYears: 'Años académicos',
    gradeLevels: 'Grados',
    sections: 'Secciones',
    enrollments: 'Matrículas',
    search: 'Buscar',
    create: 'Crear',
    refresh: 'Actualizar',
    firstName: 'Nombre',
    lastName: 'Apellido',
    email: 'Correo',
    phone: 'Teléfono',
    studentNumber: 'Número de estudiante',
    person: 'Persona',
    name: 'Nombre',
    code: 'Código',
    startDate: 'Fecha de inicio',
    endDate: 'Fecha final',
    status: 'Estado',
    sortOrder: 'Orden',
    campus: 'Campus',
    gradeLevel: 'Grado',
    academicYear: 'Año académico',
    section: 'Sección',
    capacity: 'Capacidad',
    enrollmentDate: 'Fecha de matrícula',
    newPerson: 'Nueva persona',
    newStudent: 'Crear perfil de estudiante',
    newAcademicYear: 'Nuevo año académico',
    newGradeLevel: 'Nuevo grado',
    newCampus: 'Nuevo campus',
    newSection: 'Nueva sección',
    newEnrollment: 'Nueva matrícula',
    noRecords: 'No hay registros.',
    loadError: 'No fue posible cargar los datos.',
    saveError: 'No fue posible guardar. Revisa los datos e intenta nuevamente.',
    requiredSetup: 'Crea primero los registros requeridos.',
    active: 'Activo',
    inactive: 'Inactivo',
    Planned: 'Planificado',
    Active: 'Activo',
    Closed: 'Cerrado',
    Pending: 'Pendiente',
    Withdrawn: 'Retirado',
    Transferred: 'Transferido',
    Completed: 'Completado',
    footer: 'AequorX · República Dominicana',
    notFound: 'Página no encontrada',
    routeMissing: 'La ruta solicitada no existe.',
    backHome: 'Volver al inicio',
  },
  en: {
    appSubtitle: 'School administration',
    language: 'Language',
    spanish: 'Español',
    english: 'English',
    people: 'People',
    students: 'Students',
    academicStructure: 'Academic structure',
    academicYears: 'Academic years',
    gradeLevels: 'Grade levels',
    sections: 'Sections',
    enrollments: 'Enrollments',
    search: 'Search',
    create: 'Create',
    refresh: 'Refresh',
    firstName: 'First name',
    lastName: 'Last name',
    email: 'Email',
    phone: 'Phone',
    studentNumber: 'Student number',
    person: 'Person',
    name: 'Name',
    code: 'Code',
    startDate: 'Start date',
    endDate: 'End date',
    status: 'Status',
    sortOrder: 'Sort order',
    campus: 'Campus',
    gradeLevel: 'Grade level',
    academicYear: 'Academic year',
    section: 'Section',
    capacity: 'Capacity',
    enrollmentDate: 'Enrollment date',
    newPerson: 'New person',
    newStudent: 'Create student profile',
    newAcademicYear: 'New academic year',
    newGradeLevel: 'New grade level',
    newCampus: 'New campus',
    newSection: 'New section',
    newEnrollment: 'New enrollment',
    noRecords: 'No records.',
    loadError: 'The data could not be loaded.',
    saveError: 'The record could not be saved. Review the data and try again.',
    requiredSetup: 'Create the required records first.',
    active: 'Active',
    inactive: 'Inactive',
    Planned: 'Planned',
    Active: 'Active',
    Closed: 'Closed',
    Pending: 'Pending',
    Withdrawn: 'Withdrawn',
    Transferred: 'Transferred',
    Completed: 'Completed',
    footer: 'AequorX · Dominican Republic',
    notFound: 'Page not found',
    routeMissing: 'The requested route does not exist.',
    backHome: 'Back to start',
  },
} as const;

export type TranslationKey = keyof typeof translations.es;

@Injectable({ providedIn: 'root' })
export class I18nService {
  readonly language = signal<Language>('es');

  text(key: TranslationKey): string {
    return translations[this.language()][key];
  }

  setLanguage(language: Language): void {
    this.language.set(language);
    document.documentElement.lang = language;
  }

  status(value: string): string {
    return value in translations.es
      ? this.text(value as TranslationKey)
      : value;
  }
}
