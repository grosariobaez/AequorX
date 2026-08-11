# School ERP RD
# System Design Document — SDD

**Document ID:** SDD-SERP-RD-001  
**Version:** 1.0  
**Status:** APPROVED BASELINE  
**Project:** School ERP RD  
**Market:** República Dominicana  
**Product Type:** Multi-tenant SaaS School Management ERP  
**Primary Architecture:** Modular Monolith  
**Language of Domain:** English canonical vocabulary / Spanish user experience  
**Engineering Principle:**  

> **“Make every single detail perfect, and limit the number of details.”**

---

# 1. Purpose

Este documento define el diseño funcional, lógico y arquitectónico de **School ERP RD** y constituye la referencia principal para el diseño e implementación del MVP.

El SDD define:

- límites del producto;
- arquitectura;
- módulos;
- bounded contexts;
- conceptos canónicos;
- reglas de negocio;
- flujos críticos;
- modelo de datos conceptual;
- seguridad;
- auditoría;
- multi-tenancy;
- automatización;
- integración;
- facturación electrónica;
- pagos;
- resiliencia;
- idempotencia;
- requisitos no funcionales;
- criterios de diseño;
- restricciones para implementación.

El documento no pretende describir cada clase, tabla o endpoint.

Define las decisiones necesarias para que esas implementaciones puedan crearse correctamente sin que Codex o los desarrolladores tengan que inventar comportamiento de negocio.

---

# 2. Engineering Philosophy

Todo el sistema seguirá:

> **Make every single detail perfect, and limit the number of details.**

Esto implica:

- fewer concepts, better concepts;
- fewer screens, better screens;
- fewer workflows, better workflows;
- fewer dependencies, better dependencies;
- explicit business rules;
- deterministic behavior;
- minimal cognitive load;
- configuration only where real institutional variation exists;
- automation before AI;
- auditability by design;
- security by design;
- historical integrity by design.

La arquitectura debe favorecer:

```text
Correctness
    ↓
Simplicity
    ↓
Automation
    ↓
Scalability
```

No:

```text
Complex architecture
    ↓
Features
    ↓
Business problem
```

---

# 3. Product Definition

School ERP RD es:

> **A SaaS platform that unifies the academic, administrative, financial and fiscal operations of Dominican private schools while reducing unnecessary manual work through simple, auditable and highly automated workflows.**

El objetivo principal no es sustituir cada plataforma utilizada por una institución.

El sistema debe convertirse en el **system of record operacional del colegio** para los dominios incluidos en el MVP.

---

# 4. Target Customer

## 4.1 Primary Market

Colegios privados de República Dominicana.

## 4.2 Initial Target Profile

Instituciones con:

- uno o varios campus;
- estudiantes desde niveles iniciales/primarios/secundarios según configuración;
- mensualidades recurrentes;
- matrícula;
- múltiples conceptos de cobro;
- necesidad de cumplimiento fiscal dominicano;
- profesores y personal administrativo;
- padres/tutores como usuarios externos;
- procesos actualmente fragmentados entre software, hojas de cálculo, papel, correo y mensajería.

## 4.3 Primary Product Users

- School Administrator
- Principal / Director
- Registrar
- Academic Coordinator
- Teacher
- Cashier
- Finance User
- Counselor where authorized
- Parent / Guardian
- Student where later required
- Technical Administrator

---

# 5. Product Goals

El MVP debe lograr cinco resultados.

## GOAL-01 — Single Source of Truth

Mantener una fuente confiable para:

- personas;
- estudiantes;
- relaciones familiares;
- estructura académica;
- matrícula;
- asistencia;
- calificaciones;
- obligaciones financieras;
- pagos;
- documentos fiscales.

## GOAL-02 — Reduce Administrative Work

Automatizar procesos repetitivos sin eliminar controles humanos necesarios.

## GOAL-03 — Financial Visibility

Permitir que colegio y responsables financieros conozcan correctamente:

- cargos;
- facturas;
- saldo;
- pagos;
- asignaciones;
- créditos;
- devoluciones.

## GOAL-04 — Dominican Compliance

Diseñar explícitamente para requerimientos fiscales y educativos dominicanos sin incorporar reglas regulatorias como valores rígidos.

## GOAL-05 — Excellent Core Experience

Hacer pocas operaciones, pero hacerlas excepcionalmente bien.

---

# 6. Non-Goals

El MVP NO pretende ser:

- un LMS completo;
- un sistema de videoconferencia;
- una suite completa de HR;
- un ERP contable general completo;
- un sistema de nómina;
- un sistema de activos;
- una plataforma BI corporativa;
- un sistema de chat;
- un marketplace;
- una plataforma de IA autónoma;
- una arquitectura basada en microservicios.

---

# 7. MVP Scope

## 7.1 Platform

Incluido:

- tenant management;
- organizations;
- campuses;
- authentication;
- authorization;
- roles;
- permissions;
- tenant isolation;
- audit;
- basic configuration;
- notifications.

---

# 7.2 People & Student

Incluido:

- Person;
- StudentProfile;
- EmployeeProfile cuando sea requerido;
- StudentRelationship;
- Household;
- contact information;
- responsible persons;
- authorized contacts;
- basic student identity.

---

# 7.3 Academic Structure

Incluido:

- AcademicYear;
- AcademicTerm;
- Level;
- Cycle;
- GradeLevel;
- Section;
- Subject;
- Class;
- teacher assignment where required.

---

# 7.4 Enrollment

Incluido:

- enrollment;
- enrollment history;
- section assignment;
- withdrawal;
- transfer;
- completion;
- enrollment status transitions.

---

# 7.5 Attendance

Incluido:

- attendance by exception;
- absent;
- late;
- excused;
- early departure;
- corrections;
- audit;
- guardian notifications according to policy.

---

# 7.6 Assessment

Incluido:

- assessments;
- grade entry;
- draft grades;
- publication;
- correction;
- history;
- academic policies;
- promotion-related information.

---

# 7.7 Billing & Receivables

Incluido:

- BillingAccount;
- responsible parties;
- student beneficiaries;
- ChargeType;
- Charge;
- Invoice;
- Receivable;
- Credit;
- Adjustment;
- Refund;
- balances;
- statements;
- recurring school charges.

---

# 7.8 Payments

Incluido:

- payment recording;
- payment provider integration;
- payment allocation;
- partial payments;
- multiple allocations;
- unapplied payments where required;
- reversal;
- refund;
- payment status;
- basic reconciliation.

---

# 7.9 Fiscal / DGII

Incluido:

- FiscalTreatment;
- FiscalDocument;
- FiscalDocumentType;
- e-CF boundary;
- DGII submission lifecycle;
- fiscal status;
- retry;
- rejection;
- contingency when defined by approved DGII specification;
- credit/debit corrective documents according to applicable rules.

---

# 7.10 User Experiences

Incluido:

### Administration Web

Gestión operacional completa.

### Teacher Web

Flujos optimizados para:

- clases;
- asistencia;
- calificaciones.

### Responsive Parent Portal

Inicialmente:

- student context;
- attendance information;
- grades where published;
- balance;
- charges/invoices;
- payments;
- notifications.

No native iOS/Android en MVP.

---

# 8. Explicit Post-MVP Scope

Fuera del MVP:

- General Ledger;
- Accounts Payable;
- Payroll;
- TSS;
- Procurement;
- Inventory;
- Fixed Assets;
- Budgeting;
- Native iOS;
- Native Android;
- Full LMS;
- Advanced Admissions Automation;
- Document AI;
- Predictive AI;
- AI Agents;
- Executive AI Copilot;
- Advanced BI;
- visual workflow designer;
- configurable BPM engine;
- SIGERD direct integration until specification is verified.

---

# 9. System Context

```text
                    ┌─────────────────────┐
                    │    School Staff     │
                    └──────────┬──────────┘
                               │
                    ┌──────────▼──────────┐
                    │ Administration Web │
                    └──────────┬──────────┘
                               │
         ┌───────────────┐     │     ┌────────────────┐
         │   Teachers    │─────┼────▶│ Teacher Web    │
         └───────────────┘     │     └────────────────┘
                               │
         ┌───────────────┐     │
         │ Parents       │─────┼────▶ Parent Portal
         └───────────────┘     │
                               ▼
                  ┌─────────────────────────┐
                  │      SCHOOL ERP RD      │
                  │                         │
                  │ Modular Monolith        │
                  └───────────┬─────────────┘
                              │
       ┌──────────────────────┼───────────────────────┐
       │                      │                       │
       ▼                      ▼                       ▼
     DGII               Payment Providers       Notifications
                           AZUL/CardNet           Email/Push/
                                                  WhatsApp*
```

`*` según proveedores aprobados.

---

# 10. Architecture Style

## ADR-ARCH-001

El MVP utilizará:

> **Modular Monolith + Domain-Oriented Modules + API-First + Internal Domain Events**

La solución será inicialmente un único deployable lógico.

Los módulos representan boundaries de dominio, no microservicios.

---

# 11. Why Modular Monolith

Se selecciona porque ofrece:

- menor complejidad operacional;
- transacciones más simples;
- implementación más rápida;
- debugging más sencillo;
- alta cohesión;
- boundaries explícitos;
- menor costo;
- capacidad futura de extraer módulos si existe evidencia operacional que lo justifique.

No se seleccionan microservicios anticipadamente.

---

# 12. Logical Architecture

```text
┌─────────────────────────────────────────────────────┐
│                    PRESENTATION                     │
│                                                     │
│ Admin Web       Teacher Web        Parent Portal    │
└─────────────────────────┬───────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────┐
│                   APPLICATION API                   │
│                                                     │
│ Commands | Queries | Authorization | Validation     │
└─────────────────────────┬───────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────┐
│                  DOMAIN MODULES                     │
│                                                     │
│ Identity                                            │
│ People                                              │
│ Academic Structure                                  │
│ Enrollment                                          │
│ Attendance                                          │
│ Assessment                                          │
│ Billing                                             │
│ Payments                                            │
│ Fiscal                                              │
│ Notifications                                       │
│ Audit                                               │
└─────────────────────────┬───────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────┐
│                  INFRASTRUCTURE                     │
│                                                     │
│ Database                                            │
│ External Providers                                  │
│ Background Jobs                                     │
│ Email/Push                                          │
│ Payment Adapters                                    │
│ DGII Adapter                                        │
│ Observability                                       │
└─────────────────────────────────────────────────────┘
```

---

# 13. Module Ownership

Cada concepto tendrá un único módulo propietario.

## Identity & Access

Owns:

- User
- Role
- Permission
- authentication association

## People

Owns:

- Person
- StudentProfile
- EmployeeProfile
- StudentRelationship
- Household

## Academic Structure

Owns:

- AcademicYear
- AcademicTerm
- Level
- Cycle
- GradeLevel
- Section
- Subject
- Class

## Enrollment

Owns:

- Enrollment
- enrollment transitions

## Attendance

Owns:

- Attendance records
- attendance corrections

## Assessment

Owns:

- Assessment
- Grade
- grade publication/correction
- AcademicPolicy

## Billing

Owns:

- BillingAccount
- ChargeType
- Charge
- Invoice
- Receivable
- Credit
- Adjustment

## Payments

Owns:

- Payment
- PaymentAllocation
- Refund
- provider transaction relationship

## Fiscal

Owns:

- FiscalTreatment
- FiscalDocument
- FiscalDocumentType
- DGII submission state

## Notifications

Owns:

- Notification
- NotificationTemplate
- delivery state

## Audit

Owns:

- AuditEvent

---

# 14. Canonical Vocabulary

Los siguientes conceptos son canónicos.

## Platform

- Tenant
- Organization
- Campus
- User
- Role
- Permission
- AuditEvent

## People

- Person
- StudentProfile
- EmployeeProfile
- StudentRelationship
- Household

## Academic

- AcademicYear
- AcademicTerm
- Level
- Cycle
- GradeLevel
- Section
- Subject
- Class
- Enrollment
- Attendance
- Assessment
- Grade
- AcademicPolicy

## Finance

- BillingAccount
- ChargeType
- Charge
- Invoice
- Receivable
- Payment
- PaymentAllocation
- Credit
- Refund
- Adjustment

## Fiscal

- FiscalTreatment
- FiscalDocument
- FiscalDocumentType

## Communications

- Notification
- NotificationTemplate

Sinónimos no deben introducirse sin necesidad real.

---

# 15. Person Identity Model

## ADR-DOM-001

Una persona física será representada por:

```text
Person
```

Los roles de dominio se agregan mediante perfiles o relaciones.

```text
Person
├── StudentProfile
├── EmployeeProfile
└── StudentRelationship
```

No utilizar jerarquías rígidas tipo:

```text
Teacher : Person
Parent : Person
Student : Person
```

como modelo de identidad principal.

---

# 16. Person Deduplication

Cuando una persona ya exista dentro del tenant, no debe crearse otra identidad simplemente porque asuma un nuevo rol.

Ejemplo:

```text
Person: Maria Pérez

EmployeeProfile:
Teacher

StudentRelationship:
Guardian of Student 128
```

La detección de duplicados debe utilizar atributos apropiados y evitar merges automáticos destructivos.

---

# 17. Household Model

`Household` representa agrupación familiar/demográfica.

NO representa obligación financiera.

Un estudiante puede mantener relaciones con múltiples personas y contextos familiares.

---

# 18. Billing Account Model

## ADR-DOM-002

`BillingAccount` es una entidad financiera independiente.

Puede asociarse con:

- uno o varios estudiantes;
- una o varias personas responsables;
- organización patrocinadora;
- terceros autorizados.

Ejemplos:

```text
BillingAccount A
├── Student: Jonas
└── Responsible: Parent A
```

```text
BillingAccount B
├── Student: Gabriela
├── Student: Jonas
├── Responsible: Parent A
└── Responsible: Parent B
```

El sistema no debe asumir una relación 1:1 entre familia, alumno y cuenta financiera.

---

# 19. Academic Structure

Modelo conceptual:

```text
Tenant
  ↓
Organization
  ↓
Campus
  ↓
AcademicYear
  ↓
Level
  ↓
Cycle
  ↓
GradeLevel
  ↓
Section
```

`AcademicTerm` divide el año escolar cuando corresponda.

`Subject` representa materia.

`Class` representa una oferta/contexto concreto de enseñanza de una materia.

---

# 20. Enrollment Model

## ADR-DOM-003

Enrollment es una entidad histórica.

Conceptualmente:

```text
Enrollment
├── Student
├── Organization
├── Campus
├── AcademicYear
├── GradeLevel
├── Section
├── StartDate
├── Status
├── EndDate?
└── Reason?
```

No actualizar directamente el grado del estudiante destruyendo historial.

---

# 21. Enrollment Lifecycle

Estados mínimos candidatos:

```text
Pending
Active
Withdrawn
Transferred
Completed
```

Las transiciones deberán ser explícitas.

Ejemplo:

```text
Pending → Active
Active → Withdrawn
Active → Transferred
Active → Completed
```

No permitir:

```text
Enrollment.Status = arbitraryValue
```

sin validar transición.

---

# 22. Student Lifecycle

No se modelará todo el lifecycle del estudiante mediante un único campo `StudentStatus`.

Estados como Applicant pueden pertenecer posteriormente al dominio Admissions.

Durante MVP, el estado académico relevante se deriva principalmente de Enrollment.

---

# 23. Attendance Design

## ADR-DOM-004

Attendance utilizará **attendance by exception**.

Workflow:

```text
Open Class
    ↓
All active enrolled students = Present
    ↓
Teacher marks exceptions
    ↓
Save
```

Estados candidatos:

- Present
- Absent
- Late
- Excused
- EarlyDeparture

No agregar más estados sin necesidad comprobada.

---

# 24. Attendance Workflow

```text
Teacher opens scheduled class
        ↓
System loads active enrollment roster
        ↓
Default = Present
        ↓
Teacher marks exceptions
        ↓
Validate
        ↓
Persist attendance
        ↓
AttendanceRecorded
        ↓
Notification policy evaluated
```

Correcciones posteriores deben generar auditoría.

---

# 25. Attendance Business Rules

## BR-ATT-001

Solo estudiantes con Enrollment activo aplicable a la clase pueden recibir asistencia.

## BR-ATT-002

Los estudiantes presentes no requieren marcación individual.

## BR-ATT-003

Las correcciones posteriores a la entrega inicial deben preservar el valor anterior.

## BR-ATT-004

Notificaciones por ausencia, tardanza o salida temprana dependerán de NotificationPolicy/configuración aprobada.

## BR-ATT-005

Los umbrales regulatorios de asistencia no serán magic numbers.

---

# 26. Academic Policy

## ADR-DOM-005

Las reglas académicas con variación normativa se representan mediante políticas versionables.

```text
AcademicPolicy
├── GradingPolicy
├── AttendancePolicy
└── PromotionPolicy
```

Una política puede depender de:

- academic year;
- level;
- cycle;
- grade;
- regulatory version;
- school configuration where allowed.

Valores regulatorios nunca deben encontrarse enterrados en código.

---

# 27. Assessment

`Assessment` representa una actividad o evaluación que produce resultados académicos.

Puede asociarse a:

- class;
- subject;
- academic term;
- academic policy.

El modelo exacto de ponderaciones será configurado únicamente según requisitos aprobados.

---

# 28. Grade Lifecycle

Estados:

```text
Draft
Published
Corrected
```

Conceptualmente:

```text
Teacher enters grade
        ↓
Draft
        ↓
Publish
        ↓
Published
        ↓
Correction required?
        ↓
Correct with reason
        ↓
Corrected version
```

---

# 29. Grade Integrity

Una nota publicada no se sobrescribe silenciosamente.

Cada corrección debe registrar:

- previous value;
- new value;
- person responsible;
- timestamp;
- reason;
- approval if policy requires it.

---

# 30. Billing Architecture

```text
BillingAccount
      ↓
Charge
      ↓
Invoice
      ↓
Receivable
```

El sistema distinguirá obligación económica, documento comercial/fiscal y pago.

---

# 31. Charge

`Charge` representa una obligación/cargo generado por un concepto escolar.

Ejemplos:

- tuition;
- enrollment;
- transportation;
- activity;
- books;
- other configured charge type.

Un cargo aún no es necesariamente un documento fiscal.

---

# 32. Charge Type

`ChargeType` define el significado comercial de un cargo.

Puede referenciar:

```text
FiscalTreatment
```

Nunca se infiere universalmente fiscalidad solo porque el emisor sea un colegio.

---

# 33. Invoice

`Invoice` representa la facturación comercial de una o varias obligaciones.

Debe distinguir:

- invoice lifecycle;
- payment status;
- fiscal status.

Nunca utilizar un único status para representar los tres.

---

# 34. Fiscal Document Separation

## ADR-DOM-006

`Invoice` y `FiscalDocument` son conceptos diferentes.

```text
Invoice
   ↓
Fiscal requirement determined
   ↓
FiscalDocument
   ↓
DGII
```

Esta separación permite:

- fiscal lifecycle independiente;
- rejection;
- retry;
- contingency;
- corrective fiscal documents.

---

# 35. Fiscal Treatment

Modelo:

```text
ChargeType
    ↓
FiscalTreatment
```

`FiscalTreatment` deberá representar únicamente reglas aprobadas.

No almacenar:

```text
School.IsTaxExempt
```

como atajo universal.

---

# 36. Payment Model

`Payment` representa una transferencia de valor confirmada o en proceso según su lifecycle.

Payment puede originarse mediante:

- payment provider;
- bank transfer;
- manual cashier operation if approved;
- other future mechanisms.

---

# 37. Payment Lifecycle

Modelo mínimo candidato:

```text
Pending
Confirmed
Failed
Reversed
```

`Refund` puede ser entidad/operación relacionada en lugar de utilizar `Refunded` como estado total cuando existan devoluciones parciales.

---

# 38. Payment Allocation

## ADR-DOM-007

PaymentAllocation será first-class.

Permite:

```text
1 Payment → N Receivables
N Payments → 1 Receivable
Partial Payment
Unapplied Balance
```

No modelar únicamente:

```text
Payment.InvoiceId
```

---

# 39. Payment Allocation Policy

El MVP debe permitir asignación explícita.

El sistema podrá posteriormente sugerir asignaciones mediante política.

No implementar auto-allocation irreversible sin requisito aprobado.

Candidate policies:

```text
OldestDueFirst
ExplicitSelection
ConfiguredPriority
```

La selección final debe ser trazable.

---

# 40. Credits

Credit representa valor financiero a favor disponible para aplicarse según reglas aprobadas.

No confundir con una Credit Note fiscal.

---

# 41. Refund

Refund representa devolución de dinero.

Una devolución financiera y una corrección fiscal pueden requerir operaciones separadas.

---

# 42. Financial Historical Integrity

No hard delete para:

- Invoice;
- Receivable;
- Payment;
- PaymentAllocation;
- Refund;
- FiscalDocument.

Correcciones se realizan mediante:

- cancellation;
- reversal;
- credit;
- adjustment;
- refund.

---

# 43. Monthly Billing

Los procesos recurrentes deberán ser idempotentes.

Business operation identity debe incluir suficiente contexto, por ejemplo:

```text
Tenant
BillingPeriod
Student/BillingAccount
ChargeType
```

El mismo billing run no debe producir cargos duplicados.

---

# 44. Financial Idempotency

El sistema tendrá boundaries independientes de idempotencia.

## Billing Idempotency

```text
Repeated billing execution
≠
duplicate Charge
```

## Payment Idempotency

```text
Repeated provider callback
≠
duplicate Payment
```

## Fiscal Idempotency

```text
Repeated DGII submission
≠
duplicate FiscalDocument
```

Nunca utilizar automáticamente un identificador de transporte externo como llave universal para dominios distintos.

---

# 45. Payment Provider Abstraction

```text
Payment Module
      ↓
PaymentProvider
      ├── AzulPaymentProvider
      └── CardNetPaymentProvider
```

Contrato conceptual mínimo:

- InitiatePayment
- GetPaymentStatus
- ProcessNotification
- RefundPayment

Capacidades adicionales solo cuando el provider y requisito lo permitan:

- TokenizePaymentMethod
- ChargeStoredPaymentMethod

---

# 46. Payment Provider Rule

Provider-specific DTOs, errors and transport contracts no pueden contaminar el domain model.

Adapters traducen entre:

```text
Provider Contract
        ⇅
Internal Payment Model
```

---

# 47. Payment Callback Workflow

```text
Receive provider notification
        ↓
Authenticate / verify notification
        ↓
Idempotency lookup
        ↓
Map external status
        ↓
Validate Payment transition
        ↓
Persist transaction
        ↓
PaymentConfirmed
        ↓
Application effects
```

Un callback duplicado debe ser harmless.

---

# 48. Payment Security

El sistema no almacenará:

- CVV;
- raw PAN;
- complete card credentials.

Se almacenarán únicamente tokens/references aprobados cuando corresponda.

No loggear payment secrets.

---

# 49. Fiscal Architecture

```text
Billing
    ↓
Fiscal Requirement
    ↓
Fiscal Module
    ↓
DgiiFiscalProvider
    ↓
DGII
```

El Billing module no construye directamente XML DGII.

---

# 50. Fiscal Lifecycle

Los estados exactos deben alinearse con especificación DGII aprobada.

Conceptualmente se requiere representar:

- preparation;
- signing;
- submission;
- acknowledgement;
- acceptance/rejection;
- retry;
- contingency;
- correction.

No implementar nombres de estados inventados si contradicen la especificación oficial.

---

# 51. Fiscal Operation Idempotency

Una solicitud repetida para la misma operación fiscal aprobada no debe crear un segundo documento fiscal accidentalmente.

El identificador idempotente fiscal debe representar la operación fiscal, no el RRN de un pago.

---

# 52. Fiscal Corrections

Un documento fiscal ya emitido no debe alterarse destructivamente.

Correcciones deberán utilizar el mecanismo fiscal aplicable según DGII.

---

# 53. Fiscal Configuration

El tratamiento fiscal deberá ser:

- explicit;
- traceable;
- versionable where required;
- linked to applicable business concepts.

Reglas regulatorias deben documentar:

- requirement ID;
- authoritative source;
- effective date.

---

# 54. Notifications Architecture

Modelo:

```text
Business Event
     ↓
Notification Rule
     ↓
Recipient
     ↓
Template
     ↓
Channel
     ↓
Delivery
```

Dominios no deben llamar directamente WhatsApp/email providers.

---

# 55. Notification Channels

Inicialmente se podrán soportar canales aprobados como:

- email;
- push;
- WhatsApp where provider integration is approved;
- in-app.

SMS podrá añadirse según necesidad.

---

# 56. Notifications Are Not Chat

No implementar chat bidireccional como parte del MVP.

---

# 57. Domain Events

Solo eventos con significado de negocio.

Candidatos:

```text
StudentEnrolled
EnrollmentWithdrawn
AttendanceRecorded
GradePublished
GradeCorrected
ChargeCreated
InvoiceIssued
PaymentConfirmed
PaymentAllocated
PaymentReversed
FiscalDocumentSubmitted
FiscalDocumentAccepted
FiscalDocumentRejected
```

No emitir domain events por cada cambio CRUD trivial.

---

# 58. Event Processing

En el modular monolith, los eventos podrán procesarse inicialmente in-process cuando:

- consistencia lo permita;
- resiliencia no requiera persistencia asíncrona.

Cuando una operación externa o diferida necesite garantías adicionales, podrá utilizarse procesamiento persistente/background según ADR aprobado.

---

# 59. Background Processing Candidates

Casos válidos:

- monthly billing;
- notifications;
- external submission retry;
- reconciliation;
- fiscal retries.

No agregar un scheduler/background framework sin necesidad concreta.

---

# 60. Transaction Boundaries

Una operación local crítica debe preservar atomicidad cuando corresponda.

External APIs no deben incluirse ingenuamente dentro de una transacción de base de datos abierta.

Patrón conceptual:

```text
Persist local intent/state
        ↓
Commit
        ↓
Invoke external operation
        ↓
Persist result
```

La implementación final deberá preservar idempotencia y recoverability.

---

# 61. Data Architecture

El sistema utilizará almacenamiento transaccional relacional salvo ADR que demuestre necesidad diferente.

El modelo deberá favorecer:

- referential integrity;
- explicit relationships;
- constraints;
- unique keys;
- tenant isolation;
- history;
- indexing by access pattern.

No utilizar EAV o JSON genérico como sustituto del modelado.

---

# 62. Aggregate Design

Los aggregates deberán mantenerse pequeños y basados en invariantes reales.

No construir agregados gigantescos como:

```text
Student
├── all grades
├── all invoices
├── all payments
├── all attendance
└── all documents
```

El Student 360 será una **read model/view**, no un mega-aggregate.

---

# 63. Student 360

Student 360 combinará información autorizada proveniente de múltiples módulos.

Conceptualmente:

```text
Student Identity
Enrollment
Attendance Summary
Academic Summary
Financial Context
Relationships
Notifications
```

El acceso respetará permisos.

---

# 64. Multi-Tenancy

Modelo:

```text
Platform
    ↓
Tenant
    ↓
Organization
    ↓
Campus
```

Tenant representa aislamiento comercial y de datos.

Organization representa institución.

Campus representa ubicación/unidad académica.

---

# 65. Tenant Isolation

Todas las entidades tenant-scoped deberán respetar tenant boundaries.

No confiar en `TenantId` recibido directamente desde cliente.

Tenant debe derivarse o validarse contra identidad autenticada.

---

# 66. Tenant Isolation Tests

Debe existir automated testing que intente explícitamente:

```text
Tenant A user
    ↓
Request Tenant B resource
    ↓
DENIED
```

Cross-tenant exposure es un severity-critical defect.

---

# 67. Organization and Campus

Una Organization puede tener uno o varios Campus.

Configuración puede existir:

- tenant-wide;
- organization-wide;
- campus-specific

únicamente cuando exista variación real.

---

# 68. Authentication

La arquitectura debe soportar autenticación segura basada en estándares modernos.

La selección definitiva de Identity Provider/framework será una decisión de implementación/ADR.

No construir un authentication protocol propietario.

---

# 69. Authorization

Modelo inicial:

```text
RBAC
+
resource/context checks
```

No adoptar ABAC completo anticipadamente.

Roles representan funciones comunes.

Permissions representan acciones.

Context checks restringen recursos sensibles.

---

# 70. Sensitive Records

Categorías:

## RESTRICTED

- psychology/counseling;
- health;
- highly sensitive discipline data.

## CONFIDENTIAL

- identity documents;
- financial information;
- private contact information.

Administradores técnicos no obtienen acceso automático a contenido restringido.

---

# 71. Security Principles

- least privilege;
- deny by default;
- server-side authorization;
- tenant isolation;
- MFA for privileged accounts where required;
- secure password/identity handling;
- TLS;
- encryption at rest where appropriate;
- secrets management;
- secure provider callbacks;
- dependency security;
- audit.

---

# 72. Security Boundaries

```text
Internet
   ↓
Web/Application Boundary
   ↓
Authentication
   ↓
Authorization
   ↓
Application
   ↓
Domain
   ↓
Infrastructure
   ↓
External Providers
```

Ninguna capa externa será confiable por defecto.

---

# 73. Audit Architecture

AuditEvent debe permitir representar:

```text
Tenant
Actor
Action
EntityType
EntityId
Before
After
Timestamp
Reason
CorrelationId
```

Datos grandes/sensibles podrán requerir estrategia específica para evitar duplicación insegura.

---

# 74. Mandatory Audit Scenarios

- permission changes;
- enrollment transitions;
- attendance correction;
- published grade correction;
- invoice cancellation;
- payment reversal;
- refund;
- fiscal status corrections;
- sensitive information changes.

---

# 75. Audit Integrity

Audit records no deben poder modificarse mediante operaciones normales de usuario.

Audit no sustituye application logs.

---

# 76. Configuration Architecture

Configurar únicamente variaciones reales.

Candidates:

- academic calendar;
- academic policies;
- charge types;
- fee schedules;
- fiscal treatments;
- notification preferences/templates;
- branding;
- selected provider settings.

No crear un generic configuration engine.

---

# 77. Business Rule Traceability

Reglas significativas deberán poseer identificadores.

Ejemplo:

```text
BR-ATT-001
BR-GRD-001
BR-BILL-001
BR-PAY-001
BR-FIS-001
```

Reglas regulatorias además deben registrar referencia normativa.

---

# 78. Integration Architecture

Cada integración externa deberá tener un adapter.

```text
Domain/Application Contract
          ↓
Provider Adapter
          ↓
External System
```

Candidatos MVP:

- DGII;
- AZUL;
- CardNet when approved contract exists;
- notification providers.

---

# 79. MINERD / SIGERD

Status:

```text
DEFERRED
```

No implementar:

- imaginary API;
- speculative CSV;
- speculative XML.

La arquitectura debe permitir una futura integración/export sin alterar el core domain.

---

# 80. TSS

Status:

```text
POST-MVP
```

No influye en diseño inicial fuera de evitar dead ends evidentes.

---

# 81. LMS Integration

No LMS nativo.

Future integration boundary podrá considerar estándares relevantes cuando exista requisito real.

No implementar anticipadamente LTI/OneRoster sin scope concreto.

---

# 82. API Design Principles

- API-first;
- domain terminology;
- explicit behavior;
- stable error contracts;
- validation;
- authorization;
- idempotency where required;
- pagination;
- versioning strategy only when necessary.

No endpoint por tabla.

---

# 83. Command-Oriented Operations

Para operaciones con comportamiento:

Prefer:

```text
PublishGrade
CorrectGrade
WithdrawEnrollment
AllocatePayment
ReversePayment
```

sobre CRUD genérico.

---

# 84. Query Operations

Las consultas deben favorecer read models específicos y eficientes.

No obligar al domain model a convertirse en response model.

---

# 85. Search

Initial operational search should support high-value identifiers such as:

- student name;
- student identifier;
- guardian;
- guardian phone;
- invoice number;
- payment reference.

Search deberá respetar tenant y autorización.

---

# 86. Error Model

Diferenciar:

- ValidationError
- BusinessRuleViolation
- Unauthorized
- Forbidden
- NotFound
- Conflict
- ConcurrencyConflict
- ExternalProviderUnavailable
- ExternalProviderRejected
- UnexpectedError

No exponer stack traces a clientes.

---

# 87. Concurrency

Requiere especial atención:

- payment confirmation;
- payment allocation;
- invoice operations;
- grade publication/correction;
- enrollment transitions;
- fiscal document submission.

No utilizar last-write-wins silenciosamente donde existe integridad financiera/académica.

---

# 88. Money

Los montos financieros utilizarán representación decimal exacta.

Toda cantidad monetaria debe incluir o inferir explícitamente currency según contexto aprobado.

No utilizar floating point binario.

---

# 89. Time

Distinguir:

```text
Business Date
Timestamp
```

Ejemplos:

- DueDate;
- AcademicDate;
- EnrollmentDate;
- PaymentDate;
- CreatedAt;
- UpdatedAt.

Los timestamps deben almacenarse consistentemente y presentarse en timezone apropiado.

---

# 90. Dominican Time Context

Initial business timezone:

```text
America/Santo_Domingo
```

No asumir que una fecha UTC representa directamente la fecha académica local.

---

# 91. Performance Targets

Initial targets:

### Interactive internal operations

P95 target:

```text
< 2 seconds
```

bajo perfil de carga acordado.

External-provider latency deberá separarse de internal processing cuando corresponda.

---

# 92. Availability

Initial availability target:

```text
99.9%
```

para funciones online principales, sujeto a definición operacional final.

No introducir microservices solo para alcanzar esta cifra.

---

# 93. Scalability

El sistema debe soportar horizontal/vertical scaling razonable sin modificar business domain.

No diseñar anticipadamente para millones de colegios.

---

# 94. Reliability

Financial and academic critical operations deben preferir consistency y recoverability sobre eventual convenience.

---

# 95. Resilience

Todo provider externo puede fallar.

Casos mínimos:

- timeout;
- unavailable;
- rejected request;
- duplicate response;
- delayed callback;
- malformed callback;
- temporary DGII outage.

---

# 96. Retry Rules

Retry solo cuando:

- operación sea segura;
- idempotency esté garantizada;
- error sea potencialmente transient.

No retry automático de errores permanentes.

---

# 97. Circuit Breaking / Backoff

Podrá utilizarse donde providers externos lo justifiquen.

No introducir patrones de resiliencia complejos sin necesidad operacional.

---

# 98. Offline

Offline-first no es requirement general del MVP.

Attendance offline podrá evaluarse posteriormente si evidencia operacional lo requiere.

No introducir sincronización offline anticipadamente.

---

# 99. Observability

Production-ready observability deberá incluir como mínimo:

- structured logs;
- health checks;
- metrics;
- correlation IDs;
- provider operation visibility;
- error monitoring.

Distributed tracing solo donde aporte valor.

---

# 100. Logging

Contexto útil:

```text
TenantId
UserId
CorrelationId
Module
Operation
EntityId
Provider
```

Nunca loggear:

- passwords;
- access tokens;
- private keys;
- full DGII secrets;
- PAN;
- CVV;
- excessive sensitive student content.

---

# 101. Backup

Debe existir estrategia de backup consistente con los objetivos de recuperación.

Objetivos exactos RPO/RTO deberán aprobarse antes de producción.

Initial design targets:

```text
RPO target: ≤ 15 minutes
RTO target: ≤ 4 hours
```

Estos valores son targets hasta validación operacional.

---

# 102. Disaster Recovery

La arquitectura debe permitir:

- database restoration;
- secrets restoration/reconfiguration;
- provider configuration restoration;
- application redeployment;
- audit preservation.

Procedimientos deberán probarse antes de producción.

---

# 103. Accessibility

Las aplicaciones web deberán buscar conformidad con buenas prácticas WCAG modernas.

Flujos esenciales no dependerán únicamente de color.

Forms deberán ser keyboard accessible.

---

# 104. Responsive Design

Administration, Teacher y Parent interfaces deberán funcionar correctamente en tamaños de pantalla apropiados para su audiencia.

Parent experience será mobile-responsive desde MVP.

---

# 105. UX Principle

> **One screen should have one dominant job.**

Ejemplos:

Attendance screen:

```text
Mark exceptions
```

no:

```text
Attendance + grades + messages + student editing + payments
```

---

# 106. Common Action Principle

La acción más común deberá necesitar la menor interacción posible.

Teacher attendance:

```text
Open class
→ mark exceptions
→ save
```

---

# 107. Bulk Operations

Donde la operación naturalmente sea grupal, soportar bulk workflows.

Candidatos:

- attendance;
- grade entry;
- charge generation;
- invoice generation.

Bulk operation debe reportar errores parciales con claridad.

---

# 108. Automation Architecture

Automatizaciones seguirán:

```text
Trigger
   ↓
Conditions
   ↓
Domain/Application Action
   ↓
Outcome
   ↓
Audit/Notification
```

No generic workflow engine en MVP.

---

# 109. Rules Before AI

Secuencia obligatoria:

```text
Domain Rule
    ↓
Automation
    ↓
Integration
    ↓
AI
```

AI se utiliza solo cuando reglas deterministas no resuelven adecuadamente el problema.

---

# 110. AI Boundary

MVP:

```text
NO CORE AI REQUIREMENTS
```

Future AI deberá:

- be assistive;
- respect authorization;
- be auditable;
- support human review;
- not modify regulated records autonomously.

---

# 111. Privacy

El sistema manejará información de menores.

Aplicar:

- data minimization;
- purpose limitation;
- access restriction;
- secure retention;
- secure deletion/anonymization where legally appropriate;
- audit for sensitive changes.

---

# 112. Data Minimization

No almacenar información simplemente porque “podría ser útil”.

Cada dato debe responder:

> ¿Qué proceso o requirement necesita este dato?

---

# 113. Retention

No se fijarán periodos arbitrarios sin requisito normativo/operacional.

Retention policy deberá diferenciar:

- student records;
- fiscal documents;
- financial transactions;
- audit;
- sensitive counseling/health data.

---

# 114. Deletion

Hard delete no es default.

Cada entidad debe definir lifecycle apropiado:

```text
Deactivate
Archive
Cancel
Reverse
Expire
Delete
```

---

# 115. Testing Strategy

Testing layers:

```text
Domain Unit Tests
Application Tests
Infrastructure Integration Tests
API Integration Tests
Critical End-to-End Tests
```

Testing debe enfocarse en comportamiento.

---

# 116. Required Critical Test Suites

## Security

- tenant isolation;
- unauthorized access;
- sensitive information restrictions.

## Enrollment

- history preservation;
- invalid transitions.

## Attendance

- exception behavior;
- corrections;
- notifications.

## Assessment

- publish;
- correction;
- history.

## Billing

- recurring charge idempotency;
- cancellation;
- balance.

## Payments

- partial payment;
- multi-allocation;
- duplicate callback;
- reversal;
- refund.

## Fiscal

- duplicate submission protection;
- rejection;
- retry;
- corrective lifecycle.

---

# 117. Integration Testing

Provider adapters deberán probarse con:

- contract tests;
- sandbox/test environment where available;
- simulated error responses;
- timeout;
- duplicate callback.

No depender únicamente de manual testing.

---

# 118. Acceptance Criteria

Cada requirement implementado deberá tener criterios verificables.

Formato:

```text
Given
When
Then
```

cuando sea apropiado.

Ejemplo:

```text
Given an active class with 30 enrolled students
When the teacher records two absences
Then the remaining 28 students are recorded as present
without requiring individual interaction
```

---

# 119. Definition of Done

Una feature está DONE únicamente cuando:

- requirement implemented;
- business rules enforced;
- tests passing;
- security enforced;
- tenant isolation respected;
- audit implemented where required;
- error handling implemented;
- edge cases addressed;
- acceptance criteria satisfied;
- documentation synchronized;
- no undocumented business assumptions introduced.

---

# 120. Implementation Stop Conditions

Codex/developer deberá detener únicamente la parte afectada cuando:

- business behavior is undefined;
- fiscal rule lacks evidence;
- regulatory rule is ambiguous;
- provider specification is missing;
- destructive migration is unsafe;
- architectural change requires ADR;
- security cannot be preserved.

No inventar defaults críticos.

---

# 121. Requirement Structure

Requirements deberán usar:

```text
REQ-[DOMAIN]-###
```

Ejemplos:

```text
REQ-PPL-001
REQ-ENR-001
REQ-ATT-001
REQ-GRD-001
REQ-BILL-001
REQ-PAY-001
REQ-FIS-001
REQ-SEC-001
```

---

# 122. Business Rule Structure

```text
BR-[DOMAIN]-###
```

Cada regla incluye:

- rule;
- rationale;
- source if regulatory;
- effective date if applicable.

---

# 123. ADR Structure

```text
ADR-###
Title
Status
Context
Decision
Alternatives
Consequences
```

ADRs necesarios para decisiones arquitectónicas significativas.

---

# 124. Approved ADR Baseline

## ADR-001

Use Modular Monolith for MVP.

## ADR-002

Use unified Person identity with profiles/relationships.

## ADR-003

Separate Household from BillingAccount.

## ADR-004

Enrollment is a historical entity.

## ADR-005

Attendance uses exception-based entry.

## ADR-006

Academic regulatory rules use versioned policies.

## ADR-007

Invoice and FiscalDocument are separate concepts.

## ADR-008

PaymentAllocation is first-class.

## ADR-009

Payment providers are isolated behind provider contracts.

## ADR-010

Fiscal rules are isolated behind Fiscal domain.

## ADR-011

Use explicit domain workflows instead of generic workflow engine.

## ADR-012

Rules and automation precede AI.

## ADR-013

SIGERD concrete integration is deferred until verified.

## ADR-014

Native mobile apps are post-MVP.

## ADR-015

General Ledger/AP/Payroll are post-MVP.

---

# 125. Implementation Architecture Constraints

Codex must NOT introduce without approved ADR:

- microservices;
- message broker;
- Kubernetes;
- event sourcing;
- universal CQRS architecture;
- NoSQL primary persistence;
- generic repository abstraction everywhere;
- generic workflow engine;
- generic rules engine;
- AI orchestration platform;
- service mesh.

---

# 126. Technology Selection

Este SDD no congela todavía:

- backend language/framework;
- frontend framework;
- database engine;
- hosting cloud;
- authentication provider;
- job scheduler;
- notification vendor.

Estas decisiones deberán seleccionarse durante Solution Architecture / Bootstrap mediante ADRs.

Selection criteria:

1. correctness;
2. maintainability;
3. security;
4. team productivity;
5. ecosystem maturity;
6. cost;
7. operational simplicity.

---

# 127. Recommended Repository Architecture

La estructura concreta podrá ajustarse según tecnología, pero conceptualmente deberá reflejar:

```text
/
├── AGENTS.md
├── docs/
│   ├── SDD.md
│   ├── domain/
│   ├── architecture/
│   ├── requirements/
│   └── integrations/
│
├── src/
│   ├── Platform/
│   ├── Modules/
│   │   ├── Identity/
│   │   ├── People/
│   │   ├── Academic/
│   │   ├── Enrollment/
│   │   ├── Attendance/
│   │   ├── Assessment/
│   │   ├── Billing/
│   │   ├── Payments/
│   │   ├── Fiscal/
│   │   ├── Notifications/
│   │   └── Audit/
│   └── Host/
│
└── tests/
```

No crear cientos de projects/packages anticipadamente.

---

# 128. Dependency Direction

Preferred:

```text
Presentation
      ↓
Application
      ↓
Domain
      ↑
Infrastructure implements contracts
```

Domain no depende de:

- web framework;
- database framework;
- AZUL;
- CardNet;
- DGII transport;
- email provider.

---

# 129. Database Ownership

Aunque MVP pueda utilizar una base de datos física compartida, module boundaries deberán reflejar propiedad lógica.

Un módulo no debe modificar tablas de otro módulo directamente.

---

# 130. Cross-Module Interaction

Prefer order:

1. application contract;
2. domain/application event;
3. read model;

No compartir internals arbitrariamente.

---

# 131. Read Models

Read models pueden combinar información de varios modules para experiencias como:

- Student 360;
- Parent Portal summary;
- account statement;
- teacher class roster.

Read model no cambia ownership de datos.

---

# 132. Initial Critical Workflows

Los siguientes workflows deben ser diseñados y probados antes de considerar MVP operativo.

## WF-001 Student Enrollment

```text
Select/create Person
→ StudentProfile
→ Select AcademicYear
→ GradeLevel
→ Section
→ validate
→ create Enrollment
→ StudentEnrolled
```

## WF-002 Attendance

```text
Open Class
→ load roster
→ default Present
→ mark exceptions
→ save
→ AttendanceRecorded
→ notification evaluation
```

## WF-003 Grade Publication

```text
Create/Select Assessment
→ Enter Grades
→ Save Draft
→ Validate
→ Publish
→ GradePublished
```

## WF-004 Grade Correction

```text
Select Published Grade
→ Request Correction
→ Reason
→ authorize
→ preserve previous
→ create corrected version
→ audit
```

## WF-005 Monthly Billing

```text
Select billing period
→ determine eligible accounts/students
→ generate Charges idempotently
→ generate Invoices according to billing rules
→ create Receivables
```

## WF-006 Online Payment

```text
Select obligations
→ initiate provider payment
→ redirect/authorize
→ provider callback
→ verify callback
→ idempotency
→ confirm Payment
→ allocate
→ update balances
```

## WF-007 Fiscal Document

```text
Invoice/fiscal event
→ evaluate FiscalTreatment
→ create fiscal operation
→ generate FiscalDocument
→ sign
→ submit
→ receive result
→ persist fiscal state
```

## WF-008 Fiscal Retry

```text
Pending/failed transient submission
→ verify idempotency
→ retry
→ update same fiscal operation
```

## WF-009 Refund

```text
Select eligible Payment
→ authorize refund
→ provider refund if applicable
→ record Refund
→ update financial position
→ determine fiscal correction requirement
```

---

# 133. Primary Business Invariants

## INV-001

A Person identity must not be duplicated solely because the person has multiple roles.

## INV-002

Enrollment history cannot be destroyed by changing current academic placement.

## INV-003

Published grades cannot be silently overwritten.

## INV-004

Payment cannot be duplicated from repeated provider callbacks.

## INV-005

Monthly billing cannot generate the same charge twice for the same business operation.

## INV-006

Fiscal retries cannot create duplicate fiscal documents for the same fiscal operation.

## INV-007

BillingAccount and Household are distinct concepts.

## INV-008

Financial records are corrected through explicit reversal/credit/refund/adjustment mechanisms rather than destructive deletion.

## INV-009

Cross-tenant data access is forbidden.

## INV-010

Technical administration does not automatically grant access to restricted student information.

---

# 134. External Dependency Matrix

| External System | MVP | Boundary | Current Decision |
|---|---:|---|---|
| DGII | Yes | Fiscal Provider | Required |
| AZUL | Yes | Payment Provider | Approved |
| CardNet | Conditional | Payment Provider | Implement when specification approved |
| Email Provider | Yes | Notification Provider | Vendor TBD |
| Push Provider | Optional MVP | Notification Provider | Vendor TBD |
| WhatsApp | Optional MVP | Notification Provider | Provider TBD |
| MINERD/SIGERD | No direct integration | Regulatory Boundary | Deferred |
| TSS/SUIR | No | Payroll Boundary | Post-MVP |
| Moodle | No | LMS Boundary | Future |
| Google Classroom | No | LMS Boundary | Future |

---

# 135. Major Risks

## RISK-001 Regulatory Change

Mitigation:

- versioned academic policies;
- isolated fiscal treatment;
- source traceability.

## RISK-002 Duplicate Financial Transactions

Mitigation:

- payment idempotency;
- billing idempotency;
- unique business keys;
- state transitions.

## RISK-003 Cross-Tenant Data Leakage

Mitigation:

- server-side tenant enforcement;
- database/query filtering;
- automated isolation tests.

## RISK-004 DGII Availability

Mitigation:

- isolated fiscal state;
- retry;
- idempotency;
- contingency according to approved specification.

## RISK-005 Payment Provider Failure

Mitigation:

- asynchronous confirmation;
- provider status query;
- callback verification;
- resilient processing.

## RISK-006 Overengineering

Mitigation:

> Make every single detail perfect, and limit the number of details.

No speculative infrastructure.

## RISK-007 Requirement Drift

Mitigation:

- SDD;
- business rules;
- acceptance criteria;
- ADR;
- AGENTS.md.

---

# 136. Open Non-Blocking Decisions

Los siguientes elementos pueden decidirse durante bootstrap sin reabrir el modelo de dominio:

- backend technology;
- frontend technology;
- database technology;
- cloud environment;
- Identity Provider;
- job scheduling implementation;
- notification provider;
- observability vendor;
- CI/CD system.

---

# 137. Open Regulatory/Integration Decisions

No bloquean architecture bootstrap:

- exact SIGERD interface;
- TSS API/interface;
- future LMS interoperability;
- fiscal treatment of every possible optional school product;
- detailed promotion policies for every educational level.

Cada uno está aislado detrás de policy/integration boundaries.

---

# 138. Phase Transition Gate

Antes de comenzar feature development con Codex deben existir como mínimo:

```text
AGENTS.md                  ✓
SDD.md                     ✓
Canonical Vocabulary       ✓
ADR baseline               ✓
MVP scope                  ✓
Module boundaries          ✓
Critical workflows         ✓
Security baseline          ✓
Idempotency model          ✓
```

El siguiente paso puede ser **Solution Bootstrap**.

---

# 139. Solution Bootstrap Deliverables

Antes de desarrollar funcionalidades:

1. choose approved technical stack;
2. document choices through ADRs;
3. create solution/repository structure;
4. configure CI;
5. configure tests;
6. configure formatting/linting;
7. configure secrets strategy;
8. configure application configuration;
9. establish database migration strategy;
10. establish observability baseline;
11. implement architecture fitness tests where useful.

---

# 140. Recommended Implementation Sequence

La implementación deberá ocurrir en vertical slices pequeños.

## Foundation

```text
Tenant
Identity
Authorization
Audit
```

## People

```text
Person
StudentProfile
Relationships
```

## Academic Foundation

```text
AcademicYear
GradeLevel
Section
Class
Enrollment
```

## Academic Operations

```text
Attendance
Assessment
Grade
```

## Finance Foundation

```text
BillingAccount
ChargeType
Charge
Invoice
Receivable
```

## Payments

```text
Payment
PaymentAllocation
Provider abstraction
AZUL
```

## Fiscal

```text
FiscalTreatment
FiscalDocument
DGII
```

## Parent Experience

```text
Student Summary
Attendance
Grades
Account
Payments
```

---

# 141. Vertical Slice Rule

No construir primero toda la base de datos, después todas las APIs y finalmente todas las interfaces.

Preferir:

```text
Requirement
→ Domain
→ Application
→ Persistence
→ API
→ UI
→ Tests
```

para un workflow pequeño completo.

---

# 142. First Recommended Vertical Slice

El primer slice de negocio después de Platform Foundation debería ser:

# Student Enrollment

Porque valida:

- tenant;
- person;
- student profile;
- academic structure;
- enrollment;
- authorization;
- audit;
- persistence;
- API;
- UI.

Después:

# Attendance by Exception

Esto valida la filosofía UX principal del producto.

---

# 143. Documentation Authority

Después de aprobación:

```text
Business Requirements
        ↓
ADR
        ↓
SDD
        ↓
Domain Documentation
        ↓
AGENTS.md
        ↓
Implementation
```

Cuando implementación y SDD difieran accidentalmente, la diferencia debe resolverse explícitamente.

---

# 144. Change Management

Cambios que requieren revisión de arquitectura:

- new bounded context;
- major data ownership change;
- change from modular monolith;
- new primary persistence technology;
- new regulated behavior;
- cross-module dependency changes;
- new external provider category;
- security model change;
- tenant model change.

---

# 145. Final Architecture Principle

La calidad de School ERP RD no se medirá por cantidad de código, módulos, frameworks o características.

Se medirá por:

- exactitud del dominio;
- facilidad de operación;
- integridad de datos;
- automatización real;
- cumplimiento;
- simplicidad;
- seguridad;
- capacidad de evolución.

Para cada decisión futura se aplicará:

> **Does this detail need to exist?**

Si la respuesta es no:

eliminarlo.

Si la respuesta es sí:

hacerlo excepcionalmente bien.

---

# 146. Final System Statement

School ERP RD será construido como un sistema escolar dominicano:

**simple enough to understand,  
strict enough to protect financial and academic integrity,  
modular enough to evolve,  
automated enough to eliminate unnecessary work,  
and disciplined enough that neither humans nor AI agents need to invent its business rules during implementation.**

---

# END OF SDD

**Status:** APPROVED BASELINE  
**Next Gate:** Technical Stack Selection + Solution Bootstrap ADRs  
**Coding Authorization:** Allowed only after bootstrap architecture decisions are approved.  
**Guiding Principle:**  

> **“Make every single detail perfect, and limit the number of details.”**