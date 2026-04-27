# Mapa de Rutas Origen → Destino

## Convenciones de Migración

| Convención Blazor | Convención SvelteKit |
|-------------------|---------------------|
| PascalCase `/ChangePassword` | kebab-case `/change-password` |
| Parámetros de ruta `/{param}` | Query params `?param=` o `[param]` |
| `@page` en `.razor` | Carpeta `routes/` filesystem-based |
| Parámetros opcionales `{param?}` | Query strings `?param=value` |

---

## Login y Autenticación

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 1 | `/` | `/login` | `Login.razor` | 🔓 Público | — |
| 2 | `/Login/{recoverPassword:bool}/` | `/login?recover=true` | `Login.razor` | 🔓 Público | — |
| 3 | `/ChangePassword` | `/change-password` | `ChangePassword.razor` | 🔒 Auth | Todos |

## Registro / Datos de Usuario (Enrollment)

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 4 | `/createuser` | `/enrollment/create-user` | `CreateUser.razor` | 🔓 Público | — |
| 5 | `/Residence` | `/enrollment/residence` | `ResidenceData.razor` | 🔓 Público | — |
| 6 | `/Additional` | `/enrollment/additional` | `AdditionalData.razor` | 🔓 Público | — |
| 7 | `/SuplData2` | `/enrollment/supplementary-2` | `SuplementaryData2.razor` | 🔓 Público | — |
| 8 | `/SuplData3` | `/enrollment/supplementary-3` | `SuplementaryData3.razor` | 🔓 Público | — |
| 9 | `/Photo` | `/enrollment/photo` | `Photo.razor` | 🔓 Público | — |
| 10 | `/Enrollment` | `/enrollment` | `Enrollment.razor` | 🔓 Público | — |
| 11 | `/AuthData` | `/enrollment/auth-data` | `AuthDataProcessing.razor` | 🔓 Público | — |
| 12 | `/BarCode` | `/enrollment/barcode` | `BarCode.razor` | 🔓 Público | — |
| 13 | `/child` | `/enrollment/child` | `Child.razor` | 🔓 Público | — |
| 14 | `/child/2` | `/enrollment/child?step=2` | `Child.razor` | 🔓 Público | — |

## Gestión de PINes

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 15 | `/pines/{estado?}` | `/pines?estado={estado}` | `Pines.razor` | 🔒 Auth | Admin, Auditor |
| 16 | `/pines/certificadoingreso` | `/pines/certificado-ingreso` | `CertificadoIngreso.razor` | 🔒 Auth | Admin |

## Compra de PIN (CRC)

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 17 | `/compradepin` | `/compradepin` | `Compra-de-pin.razor` | 🔒 Auth | Admin |
| 18 | `/compradepin/datosBasicos` | `/compradepin/datos-basicos` | `DatosBasicos.razor` | 🔒 Auth | Admin |
| 19 | `/compradepin/categorias` | `/compradepin/categorias` | `Categorias.razor` | 🔒 Auth | Admin |
| 20 | `/compradepin/seleccioncategorias` | `/compradepin/seleccion-categorias` | `SeleccionCategorias.razor` | 🔒 Auth | Admin |
| 21 | `/compradepin/tipotramite` | `/compradepin/tipo-tramite` | `TipoTramite.razor` | 🔒 Auth | Admin |
| 22 | `/compradepin/tramite` | `/compradepin/tramite` | `Tramite.razor` | 🔒 Auth | Admin |
| 23 | `/compradepin/tramite-crc` | `/compradepin/tramite-crc` | `TramiteCrc.razor` | 🔒 Auth | Admin |
| 24 | `/compradepin/datospersonales` | `/compradepin/datos-personales` | `DatosPersonales.razor` | 🔒 Auth | Admin |
| 25 | `/compradepin/mediosdepago` | `/compradepin/medios-de-pago` | `MediosDePago.razor` | 🔒 Auth | Admin |
| 26 | `/compradepin/Resumencompra` | `/compradepin/resumen-compra` | `ResumenCompra.razor` | 🔒 Auth | Admin |
| 27 | `/compradepin/confirmacioncompra` | `/compradepin/confirmacion-compra` | `ConfirmacionCompra.razor` | 🔒 Auth | Admin |
| 28 | `/compradepin/proceedtopayment` | `/compradepin/proceed-to-payment` | `ProceedToPayment.razor` | 🔒 Auth | Admin |
| 29 | `/compradepin/transactionprogress` | `/compradepin/transaction-progress` | `TransactionProgress.razor` | 🔒 Auth | Admin |
| 30 | `/compradepin/operaciones` | `/compradepin/operaciones` | `Operaciones.razor` | 🔒 Auth | Admin |
| 31 | `/compradepin/resumen` | `/compradepin/resumen` | `Resumen.razor` | 🔒 Auth | Admin |
| 32 | `/compradepin/pagoCuota` | `/compradepin/pago-cuota` | `PagoCuota.razor` | 🔒 Auth | Admin |
| 33 | `/compradepin/cuotasCeas` | `/compradepin/cuotas-ceas` | `CuotasCeas.razor` | 🔒 Auth | Admin |
| 34 | `/compradepin/devoluciones` | `/compradepin/devoluciones` | `Devolucion.razor` | 🔒 Auth | Admin |
| 35 | `/compradepin/gestionarPin` | `/compradepin/gestionar-pin` | `GestionarPin.razor` | 🔒 Auth | Admin |

## Compra de PIN CDA

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 36 | `/comprapincda` | `/comprapincda` | `CompraPinCDA.razor` | 🔒 Auth | Admin |
| 37 | `/compradepincda/datospersonalesCDA` | `/comprapincda/datos-personales` | `DatosPersonalesCDA.razor` | 🔒 Auth | Admin |
| 38 | `/compradepincda/categoriasCDA` | `/comprapincda/categorias` | `CategoriasCDA.razor` | 🔒 Auth | Admin |
| 39 | `/compradepincda/tramite` | `/comprapincda/tramite` | `Tramite.razor` | 🔒 Auth | Admin |
| 40 | `/compradepincda/confirmacioncompra` | `/comprapincda/confirmacion-compra` | `ConfirmacionCompraCDA.razor` | 🔒 Auth | Admin |
| 41 | `/compradepincda/resumen` | `/comprapincda/resumen` | `ResumenCDA.razor` | 🔒 Auth | Admin |
| 42 | `/compradepincda/SpinCarga` | `/comprapincda/procesando` | `SpinProceso.razor` | 🔒 Auth | Admin |

## Blog

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 43 | `/blog/{slug}` | `/compradepin/blog/[slug]` | `Articulo.razor` (CompraPin) | 🔒 Auth | Admin |
| 44 | `/blogcda` | `/comprapincda/blog` | `Blog.razor` (CDA) | 🔒 Auth | Admin |
| 45 | `/blogcda/{slug}` | `/comprapincda/blog/[slug]` | `Articulo.razor` (CDA) | 🔒 Auth | Admin |

## Configuración

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 46 | `/configuracion/` | `/configuracion` | `Configuracion.razor` | 🔒 Auth | Admin, Director |
| 47 | `/configuracion/horario-agendamiento` | `/configuracion/horario-agendamiento` | `InicioAgendamiento.razor` | 🔒 Auth | Admin, Director |
| 48 | `/configuracion/configuracion-cupos` | `/configuracion/configuracion-cupos` | `ConfiguraCupos.razor` | 🔒 Auth | Admin, Director |
| 49 | `/configuracion/configuracion-reglas` | `/configuracion/configuracion-reglas` | `ConfiguraReglas.razor` | 🔒 Auth | Admin, Director |
| 50 | `/configuracion/configurarParametrizacion` | `/configuracion/parametrizacion` | `ConfigurarParametrizacion.razor` | 🔒 Auth | Admin, Director |
| 51 | `/configuracion/horario-atencion` | `/configuracion/horario-atencion` | `HorarioAtencion.razor` | 🔒 Auth | Admin, Director |
| 52 | `/configuracion/PerfilMilicencia` | `/configuracion/perfil-milicencia` | `Perfil.razor` | 🔒 Auth | Admin |
| 53 | `/configuracion/politicaAgendamiento` | `/configuracion/politica-agendamiento` | `PoliticaAgendamiento.razor` | 🔒 Auth | Admin, Director |
| 54 | `/configuracion/agendamiento/next` | `/configuracion/agendamiento/next` | `FakeNextPage.razor` | 🔒 Auth | Admin |
| 55 | `/configuracion/dataContact` | `/configuracion/data-contact` | `DataContact.razor` | 🔒 Auth | Admin |

## Facturación

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 56 | `/configuracion/configurarFacturacion` | `/configuracion/facturacion` | `ConfigurarFacturacion.razor` | 🔒 Auth | Admin |
| 57 | `/configuracion/configurarFacturacion/estado` | `/configuracion/facturacion/estado` | `Estado.razor` | 🔒 Auth | Admin |
| 58 | `/configuracion/configurarFacturacion/emision` | `/configuracion/facturacion/emision` | `Emision.razor` | 🔒 Auth | Admin |
| 59 | `/configuracion/configurarFacturacion/articulos` | `/configuracion/facturacion/articulos` | `Articulos.razor` | 🔒 Auth | Admin |
| 60 | `/configuracion/configurarFacturacion/credenciales` | `/configuracion/facturacion/credenciales` | `Credenciales.razor` | 🔒 Auth | Admin |
| 61 | `/configuracion/configurarFacturacion/comportamiento` | `/configuracion/facturacion/comportamiento` | `Comportamiento.razor` | 🔒 Auth | Admin |
| 62 | `/configuracion/configurarFacturacion/numeracion` | `/configuracion/facturacion/numeracion` | `Numeracion.razor` | 🔒 Auth | Admin |
| 63 | `/ConsultarFacturacion/{estado?}` | `/consultar-facturacion?estado={estado}` | `ConsultarFacturacion.razor` | 🔒 Auth | Admin, Auditor |

## Agendamiento

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 64 | `/agenda` | `/agenda` | `Agenda.razor` | 🔒 Auth | Admin, Instructor |

## SuperTransporte — Centro CRC

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 65 | `/supertransporte/centro` | `/supertransporte/centro` | `CentroSuper.razor` | 🔒 Auth | Admin, Director |
| 66 | `/supertransporte/centro/informacion-basica` | `/supertransporte/centro/informacion-basica` | `InfoBasica.razor` | 🔒 Auth | Admin, Director |
| 67 | `/supertransporte/centro/profesional-salud` | `/supertransporte/centro/profesional-salud` | `InfoProfesionalesSalud.razor` | 🔒 Auth | Admin, Director |
| 68 | `/supertransporte/centro/resolucion-habilitacion` | `/supertransporte/centro/resolucion-habilitacion` | `ResolucionHabilitacion.razor` | 🔒 Auth | Admin, Director |
| 69 | `/supertransporte/centro/acreditacion-onac` | `/supertransporte/centro/acreditacion-onac` | `AcreditacionONAC.razor` | 🔒 Auth | Admin, Director |
| 70 | `/supertransporte/centro/informacion-homologado` | `/supertransporte/centro/informacion-homologado` | `InfoHomologado.razor` | 🔒 Auth | Admin, Director |
| 71 | `/supertransporte/centro/registro-reps` | `/supertransporte/centro/registro-reps` | `RegistroREPS.razor` | 🔒 Auth | Admin, Director |
| 72 | `/supertransporte/centro/interconexion-runt` | `/supertransporte/centro/interconexion-runt` | `InterconexionRUNT.razor` | 🔒 Auth | Admin, Director |
| 73 | `/supertransporte/centro/profesional-certificadores` | `/supertransporte/centro/profesional-certificadores` | `InfoProfesionalesCertificadores.razor` | 🔒 Auth | Admin, Director |
| 74 | `/supertransporte/centro/informacion-constitucion` | `/supertransporte/centro/informacion-constitucion` | `InfoConstitucion.razor` | 🔒 Auth | Admin, Director |
| 75 | `/supertransporte/centro/representante-legal-CRC` | `/supertransporte/centro/representante-legal` | `RepresentanteLegal.razor` | 🔒 Auth | Admin, Director |
| 76 | `/supertransporte/centro/infraestructura` | `/supertransporte/centro/infraestructura` | `InfoInfraestructura.razor` | 🔒 Auth | Admin, Director |
| 77 | `/supertransporte/centro/poliza` | `/supertransporte/centro/poliza` | `Poliza.razor` | 🔒 Auth | Admin, Director |

## SuperTransporte — Centro CEA

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 78 | `/supertransporte/centro-cea` | `/supertransporte/centro-cea` | `CentroSuperCEA.razor` | 🔒 Auth | Admin, Director |
| 79 | `/supertransporte/centro-cea/informacion-basica` | `/supertransporte/centro-cea/informacion-basica` | `InfoBasicaCEA.razor` | 🔒 Auth | Admin, Director |
| 80 | `/supertransporte/centro-cea/informacion-constitucion` | `/supertransporte/centro-cea/informacion-constitucion` | `InfoConstitucionCEA.razor` | 🔒 Auth | Admin, Director |
| 81 | `/supertransporte/centro-cea/informacion-propietarios` | `/supertransporte/centro-cea/informacion-propietarios` | `InfoPropietariosCEA.razor` | 🔒 Auth | Admin, Director |
| 82 | `/supertransporte/centro-cea/informacion-homologado` | `/supertransporte/centro-cea/informacion-homologado` | `InfoHomologadoCEA.razor` | 🔒 Auth | Admin, Director |
| 83 | `/supertransporte/centro-cea/resolucion-habilitacion` | `/supertransporte/centro-cea/resolucion-habilitacion` | `ResolucionHabilitacionCEA.razor` | 🔒 Auth | Admin, Director |
| 84 | `/supertransporte/centro-cea/licencia-funcionamiento` | `/supertransporte/centro-cea/licencia-funcionamiento` | `LicenciaFuncionamientoCEA.razor` | 🔒 Auth | Admin, Director |
| 85 | `/supertransporte/centro-cea/informacion-vehiculos` | `/supertransporte/centro-cea/informacion-vehiculos` | `InfoVehiculosCEA.razor` | 🔒 Auth | Admin, Director |
| 86 | `/supertransporte/centro-cea/instructor` | `/supertransporte/centro-cea/instructor` | `InstructorCEACmp.razor` | 🔒 Auth | Admin, Director |
| 87 | `/supertransporte/centro-cea/representante-legal` | `/supertransporte/centro-cea/representante-legal` | `RepresentanteLegalCEA.razor` | 🔒 Auth | Admin, Director |
| 88 | `/supertransporte/centro-cea/programas-convenio` | `/supertransporte/centro-cea/programas-convenio` | `ProgramasConvenioCEA.razor` | 🔒 Auth | Admin, Director |
| 89 | `/supertransporte/centro-cea/infraestructura` | `/supertransporte/centro-cea/infraestructura` | `InfraestructuraCEA.razor` | 🔒 Auth | Admin, Director |
| 90 | `/supertransporte/centro-cea/poliza` | `/supertransporte/centro-cea/poliza` | `PolizaCEA.razor` | 🔒 Auth | Admin, Director |
| 91 | `/supertransporte/centro-cea/certificacion-oec` | `/supertransporte/centro-cea/certificacion-oec` | `CertificacionOECCEA.razor` | 🔒 Auth | Admin, Director |

## SuperTransporte — Otros

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 92 | `/supertransporte/pqrsf` | `/supertransporte/pqrsf` | `PQRSF.razor` | 🔒 Auth | Admin, Director |
| 93 | `/ValPinCRC` | `/validar-pin-crc` | `ValidaPinCRC.razor` | 🔒 Auth | Admin |
| 94 | `/ValPinCEA` | `/validar-pin-cea` | `ValidaPinCEA.razor` | 🔒 Auth | Admin |

## Reportes y Otros

| # | Ruta Blazor (Origen) | Ruta SvelteKit (Destino) | Archivo Origen | Acceso | Roles |
|---|---------------------|-------------------------|----------------|--------|-------|
| 95 | `/reportes` | `/reportes` | `ReportePBI.razor` | 🔒 Auth | Admin, Auditor |
| 96 | `/Reject` | `/reject` | `Reject.razor` | 🔓 Público | — |
| 97 | `/Finger` | `/fingerprints` | `Fingerprints.razor` | 🔓 Público | — |
| 98 | `/Success` | `/success` | `Success.razor` | 🔓 Público | — |
| 99 | `/fetchdata` | _(eliminar — demo)_ | `FetchData.razor` | — | — |

## Rutas API (SvelteKit)

| # | Ruta SvelteKit | Método | Propósito |
|---|---------------|--------|-----------|
| A1 | `/api/auth/logout` | `POST` | Cerrar sesión (eliminar cookie) |
| A2 | `/api/auth/login` | `POST` | _(futuro)_ Autenticar usuario |
| A3 | `/api/auth/refresh` | `POST` | _(futuro)_ Refrescar token |

## Página Especial

| # | Ruta SvelteKit | Propósito |
|---|---------------|-----------|
| S1 | `/403-unauthorized` | Página de acceso denegado |
| S2 | `+error.svelte` | Página de error global |

---

## Notas de Migración

1. **Parámetros opcionales** como `{estado?}` se migran a query strings (`?estado=activos`)
2. **Parámetros de ruta** como `{slug}` se migran a segmentos dinámicos SvelteKit (`[slug]`)
3. **Rutas CDA** se consolidan bajo `/comprapincda/` (se elimina la inconsistencia `compradepincda`)
4. **PascalCase** se normaliza a kebab-case consistentemente
5. **`/fetchdata`** es una página de demo de Blazor y se elimina
6. **Enrollment** se agrupa bajo `/enrollment/` como sub-rutas lógicas
