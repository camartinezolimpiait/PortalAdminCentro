using Microsoft.AspNetCore.Components;

using System.Threading.Tasks;
using portalAdministrativoSISEC.Entidades.Pago;
using portalAdministrativoSISEC.Enum.PortalAdministrativo;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using portalAdministrativoSISEC.Application.Data.CompraPin.Wompi;
using portalAdministrativoSISEC.Entidades.Pago.Wompi;
using System;
using System.Collections.Generic;
using System.Linq;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Request;
using static portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Request.ReferenciaBancolombiaCdaWompi;
using portalAdministrativoSISEC.Entidades.Pago.Wompi.CDA.Response;
using portalAdministrativoSISEC.Application.Data.CompraPin;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.ComfirmacionCompraCDA
{
	public partial class ConfirmacionCompraCDA
	{
		#region Inyeccion Dependencias

		[Inject]
		public IMiLicenciaService MiLicenciaService { get; set; }

		#endregion Inyeccion Dependencias

		#region Variables

		[Parameter]
		public EventCallback<PagoPinCDA> PagoPinChanged { get; set; }

		[Parameter]
		public EventCallback VolverDatosPersonales { get; set; }
		
		[Parameter]
		public EventCallback<int> PasosCompraPinChanged { get; set; }

		[Parameter]
		public PagoPinCDA PagoPin { get; set; }

		[Parameter]
		public int PasosCompraPin { get; set; }

		[Parameter]
		public string PinGenerado { get; set; }

		private bool ValidacionManual { get; set; }
		private string AcceptanceToken { get; set; } = "";
		private int IdOrigenPin { get; set; }
		private int TipoReferenciaActual { get; set; }
		private string PdfWompi { get; set; } = "";

		public bool MostrarResumenCompra { get; set; }

		public bool MostrarSpin { get; set; }

		private CostoCuotaCDA CuotaSeleccionada = new();
		public int pasosCompraPin = 0;

		#endregion Variables

		#region Methods

		protected override async Task OnInitializedAsync()
		{
			switch (PagoPin.TipoRecaudoCtrl)
			{
				case 4:
					TipoReferenciaActual = (int)EnumTipoReferencia.PinDirecto;
					await PagoElectronico();
					break;
				//Bancolombia
				case 13:
					TipoReferenciaActual = (int)EnumTipoReferencia.BancolombiaWompi;

					await CargarTycWompi();
					await PagoElectronico();
					break;

				case 14:
					await PagoElectronico();
					break;

				case 15:
					await PagoElectronico();
					break;

				default:
					await PagoElectronico();
					break;
			}
			MostrarResumenCompra = false;
		}

		private async Task CargarTycWompi()
		{
			TokenAceptacion tokenAceptacion = await MiLicenciaService.ObtenerTokenAceptacion(PagoPin.ClienteCompra);

			PdfWompi = tokenAceptacion?.Permalink;
			AcceptanceToken = tokenAceptacion?.Acceptance_token;
		}

		private async Task PagoElectronico()
		{
			IdOrigenPin = (int)EnumOrigenPin.PinesOlimpia;
			if (PagoPin.TipoRecaudoCtrl == (int)EnumTipoPago.PSE && PagoPin.TipoBancoPSE != null)
			{
				TipoReferenciaActual = (int)EnumTipoReferencia.Electronica;
			}

			await Task.FromResult(true);
		}

		public async Task PagarReferenciaPinDirecto()
		{
			ValidacionManual = false;
            ReferenciaBancolombiaCdaWompi.DataCda data = new()
			{

				Apellidos = PagoPin.Usuario.Apellido,
				CorreoElectronico = PagoPin.Usuario.Correo,
				NombreCentro=PagoPin.CentroSeleccionado.Nombre,
				DireccionCentro=PagoPin.CentroSeleccionado.Direccion,
				DispersionAliado = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Banco),
				DispersionAns = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Ansv),
				DispersionCrc = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.CDA),
				DispersionSicov = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Sicov),
				FechaNacimiento = PagoPin.Usuario.TipoDocumento == 3 ? this.CalcularEdad() : "2000-01-01",// NO EXISTE FORMULARIO PARA ASIGNAR POR DEFECTO ES 1 REV: PBI-116637
                IdConvenio = (int)EnumOrigenPin.PinDirecto, //await ObtenerConvenioMedioPago(), SE COLOCA EL DE WOMPI PARA PODER PROBAR
				IdOrigenCotizacion = (int)EnumOrigenCotizacion.PortalAdministrativo,
				IdRunt = $"{PagoPin.CentroSeleccionado.CodigoRUNT}",
				Nombres = PagoPin.Usuario.Nombre,
				NumeroIdentificacion = PagoPin.Usuario.NumDocumento,
				Sexo = 1, // NO EXISTE FORMULARIO PARA ASIGNAR POR DEFECTO ES 1 REV: PBI-116637
                TelefonoContacto = $"{PagoPin.Usuario.Celular}",
				TipoIdentificacion = PagoPin.Usuario.TipoDocumento ?? 0,
				InfoVehiculo = new InfoVehiculo() 
				{
				 NumPlaca= PagoPin.PlacaVehiculo,
				 AnioVehiculo = obtenerAnioXNum(PagoPin.EdadVehiculo),
				 IdTipoVehiculo =PagoPin.CategoriaVehiculo
                },
                Tramite = new TramiteCda()
                {
                    Categoria1 = 1,
                    Categoria2 = 0,
                    CodigoCategoria1 = PagoPin.Categoria1 ?? "",
                    CodigoCategoria2 = PagoPin.Categoria2 ?? "",
                    IdTramite1 = 1,
                    IdTramite2 = PagoPin.TipoTramite2 ?? 0
                },
				ValorTransaccion = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.ValorTotal)
			};

            RequestCreacionPinCDA referenciaCDA = new()
			{
				IdCliente = PagoPin.ClienteCompra,
				Data = data,
			};

			mostrarMsgSpin();
			ResponseCdaWompi responseReferenciaCDA = await MiLicenciaService.GenerarReferenciaPinCDA(referenciaCDA);
			ocultarMsgSpin();

			if (responseReferenciaCDA?.MensajeRespuestaField == "Ok")
			{
				//Validar que se va a hacer con la respuesta de Wompi
				PagoPin.PinGenerado = responseReferenciaCDA.PinField;
				await PagoPinChanged.InvokeAsync(PagoPin);
				IrAResumenCompra();

				var referencia = new RequestNotificacionCDA()
				{

					Pin = responseReferenciaCDA.PinField,
					IdRunt = referenciaCDA.Data.IdRunt,
					NumeroIdentificacion=referenciaCDA.Data.NumeroIdentificacion,
					TipoIdentificacion= referenciaCDA.Data.TipoIdentificacion,
					CorreoElectonico = referenciaCDA.Data.CorreoElectronico,
				    Centro= referenciaCDA.Data.NombreCentro,
					Direccion = referenciaCDA.Data.DireccionCentro
				};
                await MiLicenciaService.ConstruirCorreoCDA(referencia);

			}
			else
			{
				await MiLicenciaService.ShowNotificacion(NotificationStatus.Warning,
					$"La referencia no se genero correctamente {responseReferenciaCDA?.MensajeRespuestaField}");
			}
		}

		public async Task<List<Cuota>> SetCoutas()
		{
			List<Cuota> totalCuotas = new();

			if (PagoPin.Cuotas == 0)
			{
				totalCuotas.Add(new Cuota()
				{
					DispersionAliado = 0,
					ValorTransaccion = 0
				});
			}
			else
			{
				CuotaSeleccionada = PagoPin.CostoCuotas?.FirstOrDefault(x => x.NumeroCuotas == PagoPin.Cuotas);

				for (int i = 0; i < PagoPin.Cuotas; i++)
				{
					totalCuotas.Add(new Cuota()
					{
						DispersionAliado = (int)Math.Round(PagoPin.ValorDiscriminadoCotizacion.Banco),
						ValorTransaccion = (int)Math.Round((decimal)CuotaSeleccionada?.ValorCuota)
					});
				}
			}

			return await Task.FromResult(totalCuotas);
		}

		public async Task<int> ObtenerConvenioMedioPago()
		{
			if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CDA)
			{
				if (!string.IsNullOrEmpty(PagoPin.TipoBancoPSE))
				{
					//PSE por defecto origen Olimpia = 9
					return await Task.FromResult((int)EnumOrigenPin.PinesOlimpia);
				}
				else if (PagoPin.TipoPagoEfectivo != null)
				{
					//Punto Pago CEA = 8
					return await Task.FromResult((int)EnumOrigenPin.PuntoPagoCrc);
				}
				else if (TipoReferenciaActual == (int)EnumTipoReferencia.BancolombiaWompi)
				{
					return await Task.FromResult((int)EnumOrigenPin.BancolombiaWompi);
				}
				else if (TipoReferenciaActual == (int)EnumTipoReferencia.PinDirecto)
				{
					return await Task.FromResult((int)EnumOrigenPin.PinDirecto);
				}
			}

			if (PagoPin.TipoPagoEfectivo != null)
			{
				//Medio pago efectivo, para corresponsal bancario bancolombia de deja el origen Olimpia = 9
				return await Task.FromResult(
					PagoPin.TipoPagoEfectivo == (int)EnumOrigenPin.CorresponsalBancolombia
						? (int)EnumOrigenPin.PinesOlimpia
						: (int)PagoPin.TipoPagoEfectivo);
			}
			else if (TipoReferenciaActual == (int)EnumTipoReferencia.BancolombiaWompi)
			{
				return await Task.FromResult((int)EnumOrigenPin.BancolombiaWompi);
			}
			else
			{
				//PSE por defecto origen Olimpia = 9
				return await Task.FromResult((int)EnumOrigenPin.PinesOlimpia);
			}
		}

		public async Task<ReferenciaBancolombiaCdaWompi.TramiteCda> SetTramite() // Remover 
		{
            ReferenciaBancolombiaCdaWompi.TramiteCda tramite = new()
			{
				Categoria1 = 1,
				Categoria2 = 0,
				CodigoCategoria1 = PagoPin.Categoria1,
				CodigoCategoria2 = PagoPin.Categoria2 ?? "",
				IdTramite1 = PagoPin.TipoTramite ?? 1,
				IdTramite2 = PagoPin.TipoTramite2 ?? 0
			};

			if (PagoPin.ClienteCompra == (int)EnumTipoCliente.CDA)
			{
				tramite.Categoria1 = PagoPin.CategoriasCea.FirstOrDefault(x => x.Codigo == PagoPin.Categoria1)?.IdCategoria ?? 0;

				tramite.Categoria2 = PagoPin.CategoriasCea.FirstOrDefault(x => x.Codigo == PagoPin.Categoria2)?.IdCategoria ?? 0;
			}
			else
			{
				tramite.Categoria1 = PagoPin.CategoriasCrc.FirstOrDefault(x => x.Codigo == PagoPin.Categoria1)?.IdCategoria ?? 0;

				tramite.Categoria2 = PagoPin.CategoriasCrc.FirstOrDefault(x => x.Codigo == PagoPin.Categoria2)?.IdCategoria ?? 0;
			}

			return await Task.FromResult(tramite);
		}

		public async Task Volver()
		{
			await VolverDatosPersonales.InvokeAsync();
		}


			private void IrAResumenCompra()
		{
			MostrarResumenCompra = true;
			StateHasChanged();
		}

		private void mostrarMsgSpin()
		{
			MostrarSpin = true;
			StateHasChanged();
		}

		private void ocultarMsgSpin()
		{
			MostrarResumenCompra = false;
			MostrarSpin = false;
			StateHasChanged();
		}

		public void NavegarResumen(int paso)
		{
			pasosCompraPin = paso;
			StateHasChanged();
			PasosCompraPinChanged.InvokeAsync(paso);
		}

        public int obtenerAnioXNum(int number)
        {
            int currentYear = DateTime.Now.Year;
            return currentYear - number;
        }

		public string CalcularEdad() 
		{
			DateTime fechaNacimiento= DateTime.Today.AddYears(-17);
			string formatoFecha = fechaNacimiento.ToString("yyyy-MM-dd");
			return formatoFecha;
		}
        #endregion Methods
    }
}

