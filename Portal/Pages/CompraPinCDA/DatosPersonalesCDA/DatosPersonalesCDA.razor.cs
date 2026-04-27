using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Enum;
using portalAdministrativoSISEC.Application.Contracts.MiLicencia;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using portalAdministrativoSISEC.Application.Data.CompraPin.CDA;
using portalAdministrativoSISEC.Application.Data.CompraPin;
using portalAdministrativoSISEC.Pages.CompraPinCDA.Models;

namespace portalAdministrativoSISEC.Pages.CompraPinCDA.DatosPersonalesCDA
{
	public partial class DatosPersonalesCDA
	{

		#region Variables
		[Inject]
		private IMiLicenciaService _miLicenciaService { get; set; }

		[Parameter]
		public PagoPinCDA PagoPinCda { get; set; }

		[Parameter]
		public EventCallback<bool> OnFormValidChanged { get; set; }

		[Parameter]
		public EventCallback<PagoPinCDA> PagoPinChanged { get; set; }

		public List<TipoDocumentoPtesaDTO> ListaDocumentos { get; set; } = new();

        private bool isLoading = false;

        public int EdadVehiculo { get; set; }
		private int edadAnterior;

		public event Action OnRequestSubmit;

		private DatosPersonalesModel DatosPersonalesModel = new();
		private ValidationMessageStore MessageStore;
		private EditContext editContext;
		private string MensajeErrorDocumento;
		private Dictionary<string, bool> touchedFields = new Dictionary<string, bool>();
		#endregion

		#region Metodos
		protected override async Task OnInitializedAsync()
		{
			EdadVehiculo = PagoPinCda.EdadVehiculo;

			DatosPersonalesModel = new DatosPersonalesModel
			{
				Correo = "",
				Nombre = "",
				Apellidos = "",
				Documento = ""
			};
			editContext = new EditContext(DatosPersonalesModel);
			await ObtenerTiposDocumento();
			MessageStore = new ValidationMessageStore(editContext);
			await GetDataForm();
		}



		private async Task HandleInputChange(string fieldName, ChangeEventArgs e)
		{
			switch (fieldName)
			{
				case "correo":
					DatosPersonalesModel.Correo = e.Value.ToString();
					break;

				case "celular":
					DatosPersonalesModel.Celular = e.Value.ToString();
					break;

				case "nombre":
					DatosPersonalesModel.Nombre = e.Value.ToString();
					break;

				case "apellidos":
					DatosPersonalesModel.Apellidos = e.Value.ToString();
					break;

				case "documento":
					DatosPersonalesModel.Documento = e.Value.ToString();
					ValidarDocumento();
					break;

				case "tipoDocumento":
					DatosPersonalesModel.TipoDocumento = int.Parse(e.Value.ToString());
					ValidarDocumento();
					break;
			}

			await NotifyValidationStateChanged();
		}

		private void ValidarDocumento()
		{
			MensajeErrorDocumento = null;
			var fieldIdentifier = new FieldIdentifier(DatosPersonalesModel, nameof(DatosPersonalesModel.Documento));
			MessageStore.Clear(fieldIdentifier);
			switch (DatosPersonalesModel.TipoDocumento)
			{
				case 1: // Cédula de Ciudadanía
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "La Cédula de Ciudadanía debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 2: // Cédula de Extranjería
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "La Cédula de Extranjería debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 3: // Tarjeta de Identidad
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "La Tarjeta de Identidad debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 4: // Nit
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "El NIT debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 5: // Pasaporte
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsLetterOrDigit))
					{
						MensajeErrorDocumento = "El Pasaporte debe tener mínimo 6 caracteres y acepta letras y números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 10: // Contraseña Cédula de Ciudadanía
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "La Contraseña de la Cédula de Ciudadanía debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				case 11: // Contraseña Cédula de Extranjería
					if (DatosPersonalesModel.Documento.Length < 6 || !DatosPersonalesModel.Documento.All(char.IsDigit))
					{
						MensajeErrorDocumento = "La Contraseña de la Cédula de Extranjería debe tener mínimo 6 dígitos y solo acepta números.";
						MessageStore.Add(fieldIdentifier, MensajeErrorDocumento);
					}
					break;

				default:

					break;
			}

			editContext?.NotifyValidationStateChanged();
		}

		private async Task NotifyValidationStateChanged()
		{
			var isValid = editContext.Validate(); // Esto valida el contexto de edición y devuelve true si es válido.
			await OnFormValidChanged.InvokeAsync(isValid);
			StateHasChanged();
			if (isValid)
			{
				await HandleValidSubmit();
			}
		}

		private async Task HandleValidSubmit()

		{
			if (PagoPinCda.Usuario == null)
			{
				PagoPinCda.Usuario = new portalAdministrativoSISEC.Application.Data.CompraPin.CDA.DatosBasicosCDA();
			}
			await obtenerDescripcionTipoDoc();
			PagoPinCda.Usuario.Nombre = DatosPersonalesModel.Nombre;
			PagoPinCda.Usuario.Apellido = DatosPersonalesModel.Apellidos;
			PagoPinCda.Usuario.Correo = DatosPersonalesModel.Correo;
			PagoPinCda.Usuario.TipoDocumento = DatosPersonalesModel.TipoDocumento;
			PagoPinCda.Usuario.NumDocumento = DatosPersonalesModel.Documento;
			PagoPinCda.Usuario.Celular = long.Parse(DatosPersonalesModel.Celular);

			await ActuPin();
		}

		private async Task obtenerDescripcionTipoDoc()
		{
			if (ListaDocumentos != null && ListaDocumentos.Any())
			{

				string tipoDocumentoString = DatosPersonalesModel.TipoDocumento?.ToString();

				var descripciondocumento = ListaDocumentos.FirstOrDefault(doc => doc.IdTipoSisec == tipoDocumentoString);

				if (descripciondocumento != null)
				{
					PagoPinCda.Usuario.TipoDocumentoDescpcion = descripciondocumento.CodigoACH;
				}
				else
				{
					PagoPinCda.Usuario.TipoDocumentoDescpcion = "CC";
				}
			}
		}

		private async Task ActuPin()
		{
			await _miLicenciaService.ShowNotificacion(NotificationStatus.Success, "Informacion diligenciada con exito");
			await PagoPinChanged.InvokeAsync(PagoPinCda);
		}

		private async Task ObtenerTiposDocumento()
		{

			isLoading = true;
            var GetListaDocumentos = await _miLicenciaService.ObtenerTipoDocumentos();
			isLoading = false;
			ListaDocumentos = GetListaDocumentos.Where(doc => doc.IdTipoSisec == "3" || doc.IdTipoSisec == "1" || doc.IdTipoSisec == "2" || doc.IdTipoSisec == "5" || doc.IdTipoSisec == "13").Select(doc => new TipoDocumentoPtesaDTO
			{
				IdTipoSisec = doc.IdTipoSisec,
				Nombre = doc.Nombre, 
				TipoDocRunt = doc.TipoDocRunt,
				VisualizarCea = doc.VisualizarCea,
				VisualizarCrc = doc.VisualizarCrc,
				EdadMinima = doc.EdadMinima,
				EdadMaxima = doc.EdadMaxima,
				CodigoACH = doc.CodigoACH	
			}).ToList();
			var permitido = ListaDocumentos.Any(tipo => tipo.IdTipoSisec == PagoPinCda.Usuario.TipoDocumento?.ToString());
			if (!permitido && PagoPinCda.Usuario.TipoDocumento != 0)
			{
				PagoPinCda.Usuario.TipoDocumento = null;
				DatosPersonalesModel.TipoDocumento = null;
				MessageStore?.Clear();

				await NotifyValidationStateChanged();
				await _miLicenciaService.ShowNotificacion(NotificationStatus.Warning, " Seleccione nuevamente el tipo de documento, esto debido al cambio de edad realizado.");
			}

		}

		private async Task GetDataForm()
		{
			if (PagoPinCda.Usuario.Apellido != "" && PagoPinCda.Usuario.Nombre != "" && PagoPinCda.Usuario.Correo != "" && PagoPinCda.Usuario.NumDocumento != "" && PagoPinCda.Usuario.Celular != 0 && PagoPinCda.Usuario.TipoDocumento != 0)
			{
				DatosPersonalesModel.Correo = PagoPinCda.Usuario.Correo;
				DatosPersonalesModel.Nombre = PagoPinCda.Usuario.Nombre;
				DatosPersonalesModel.Apellidos = PagoPinCda.Usuario.Apellido;
				DatosPersonalesModel.TipoDocumento = PagoPinCda.Usuario.TipoDocumento;
				DatosPersonalesModel.Documento = PagoPinCda.Usuario.NumDocumento;
				DatosPersonalesModel.Celular = PagoPinCda.Usuario.Celular.ToString();
				await NotifyValidationStateChanged();
			}
		}
		//marcar campos tocados 
		private void OnBlur(string fieldName)
		{
			if (!touchedFields.ContainsKey(fieldName))
			{
				touchedFields[fieldName] = true;
			}
		}
		//verificar si un campo ha sido tocado
		private bool isFieldTouched(string fieldName)
		{
			return touchedFields.ContainsKey(fieldName) && touchedFields[fieldName];
		}
		#endregion
	}
}


