// Inicio código generado por GitHub Copilot
using Bunit;
using Microsoft.AspNetCore.Components;
using portalAdministrativoSISEC.Pages.Facturacion.Modal;
using Xunit;

namespace TestUnitPortalAdministrativoSisec.Facturacion
{
    /// <summary>
    /// Pruebas unitarias para el componente ModalFacturacion.
    /// Valida el comportamiento del modal en diferentes estados y la interacción con botones.
    /// </summary>
    public class ModalFacturacionTest : TestContext
    {
        #region MostrarModal = 1 - Modal de Confirmación

        /// <summary>
        /// Verifica que el modal muestre "Anular solicitud" (singular) cuando Total = 1.
        /// </summary>
        [Fact]
        public void MostrarModal1_ConTotal1_MuestraTituloSingular()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                       .Add(p => p.MostrarModal, 1)
              .Add(p => p.Total, 1));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Anular solicitud", titulo.TextContent);

            var mensaje = cut.FindAll("p").First();
            Assert.Contains("la solicitud", mensaje.TextContent);
        }

        /// <summary>
        /// Verifica que el modal muestre "Anular solicitudes" (plural) cuando Total > 1.
        /// </summary>
        [Fact]
        public void MostrarModal1_ConTotalMayorA1_MuestraTituloPlural()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                 .Add(p => p.MostrarModal, 1)
                 .Add(p => p.Total, 5));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Anular solicitudes", titulo.TextContent);

            var mensaje = cut.FindAll("p").First();
            Assert.Contains("las solicitudes", mensaje.TextContent);
        }

        /// <summary>
        /// Verifica que el modal de confirmación muestre el ícono de advertencia.
        /// </summary>
        [Fact]
        public void MostrarModal1_MuestraIconoAdvertencia()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                   .Add(p => p.MostrarModal, 1)
                  .Add(p => p.Total, 1));

            // Act & Assert
            var iconoWarning = cut.Find(".bg-warning\\/20");
            Assert.NotNull(iconoWarning);
        }

        /// <summary>
        /// Verifica que al hacer clic en el botón "Aceptar" se invoque el callback OnAceptar.
        /// </summary>
        [Fact]
        public void MostrarModal1_ClickAceptar_InvocaCallbackOnAceptar()
        {
            // Arrange
            var aceptarInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
     .Add(p => p.MostrarModal, 1)
       .Add(p => p.Total, 1)
           .Add(p => p.OnAceptar, EventCallback.Factory.Create(this, () => aceptarInvocado = true)));

            // Act
            var botonAceptar = cut.FindAll("button").First(b => b.TextContent.Contains("Aceptar"));
            botonAceptar.Click();

            // Assert
            Assert.True(aceptarInvocado);
        }

        /// <summary>
        /// Verifica que al hacer clic en el botón "Cancelar" se invoque el callback OnCancelar.
        /// </summary>
        [Fact]
        public void MostrarModal1_ClickCancelar_InvocaCallbackOnCancelar()
        {
            // Arrange
            var cancelarInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                .Add(p => p.MostrarModal, 1)
     .Add(p => p.Total, 1)
    .Add(p => p.OnCancelar, EventCallback.Factory.Create(this, () => cancelarInvocado = true)));

            // Act
            var botonCancelar = cut.FindAll("button").First(b => b.TextContent.Contains("Cancelar"));
            botonCancelar.Click();

            // Assert
            Assert.True(cancelarInvocado);
        }

        /// <summary>
        /// Verifica que al hacer clic en el botón X (cerrar) se invoque el callback OnCerrar.
        /// </summary>
        [Fact]
        public void MostrarModal1_ClickBotonX_InvocaCallbackOnCerrar()
        {
            // Arrange
            var cerrarInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                 .Add(p => p.MostrarModal, 1)
              .Add(p => p.Total, 1)
             .Add(p => p.OnCerrar, EventCallback.Factory.Create(this, () => cerrarInvocado = true)));

            // Act
            var botonCerrar = cut.Find("button.btn-circle");
            botonCerrar.Click();

            // Assert
            Assert.True(cerrarInvocado);
        }

        #endregion MostrarModal = 1 - Modal de Confirmación

        #region MostrarModal = 2 - Modal de Éxito

        /// <summary>
        /// Verifica que el modal de éxito muestre el mensaje correcto cuando Total = 1.
        /// </summary>
        [Fact]
        public void MostrarModal2_ConTotal1_MuestraMensajeExitoSingular()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                    .Add(p => p.MostrarModal, 2)
          .Add(p => p.Total, 1));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Anular solicitud", titulo.TextContent);

            var mensaje = cut.FindAll("p").First();
            Assert.Contains("Solicitud anulada", mensaje.TextContent);
            Assert.Contains("exitosamente", mensaje.TextContent);
        }

        /// <summary>
        /// Verifica que el modal de éxito muestre el mensaje correcto cuando Total > 1.
        /// </summary>
        [Fact]
        public void MostrarModal2_ConTotalMayorA1_MuestraMensajeExitoPlural()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
       .Add(p => p.MostrarModal, 2)
            .Add(p => p.Total, 3));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Anular solicitudes", titulo.TextContent);

            var mensaje = cut.FindAll("p").First();
            Assert.Contains("Solicitudes anuladas", mensaje.TextContent);
        }

        /// <summary>
        /// Verifica que el modal de éxito muestre el ícono de éxito (checkmark verde).
        /// </summary>
        [Fact]
        public void MostrarModal2_MuestraIconoExito()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
           .Add(p => p.MostrarModal, 2)
           .Add(p => p.Total, 1));

            // Act & Assert
            var iconoSuccess = cut.Find(".bg-success\\/20");
            Assert.NotNull(iconoSuccess);
        }

        /// <summary>
        /// Verifica que al hacer clic en "Aceptar" se invoque el callback OnAceptarAnulacion.
        /// </summary>
        [Fact]
        public void MostrarModal2_ClickAceptar_InvocaCallbackOnAceptarAnulacion()
        {
            // Arrange
            var aceptarAnulacionInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                        .Add(p => p.MostrarModal, 2)
                     .Add(p => p.Total, 1)
            .Add(p => p.OnAceptarAnulacion, EventCallback.Factory.Create(this, () => aceptarAnulacionInvocado = true)));

            // Act
            var botonAceptar = cut.FindAll("button").First(b => b.TextContent.Contains("Aceptar"));
            botonAceptar.Click();

            // Assert
            Assert.True(aceptarAnulacionInvocado);
        }

        #endregion MostrarModal = 2 - Modal de Éxito

        #region MostrarModal = 3 - Modal de Error

        /// <summary>
        /// Verifica que el modal de error muestre el título y mensaje correcto.
        /// </summary>
        [Fact]
        public void MostrarModal3_MuestraMensajeError()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
      .Add(p => p.MostrarModal, 3));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Error al anular", titulo.TextContent);

            var mensajes = cut.FindAll("p");
            Assert.Contains(mensajes, p => p.TextContent.Contains("Ocurrió un error"));
            Assert.Contains(mensajes, p => p.TextContent.Contains("inténtalo nuevamente"));
        }

        /// <summary>
        /// Verifica que el modal de error muestre el ícono de error (X roja).
        /// </summary>
        [Fact]
        public void MostrarModal3_MuestraIconoError()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
        .Add(p => p.MostrarModal, 3));

            // Act & Assert
            var iconoError = cut.Find(".bg-error\\/20");
            Assert.NotNull(iconoError);
        }

        /// <summary>
        /// Verifica que el modal de error muestre el correo de soporte.
        /// </summary>
        [Fact]
        public void MostrarModal3_MuestraCorreoSoporte()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
  .Add(p => p.MostrarModal, 3));

            // Act & Assert
            var enlaceCorreo = cut.Find("a[href^='mailto:']");
            Assert.Contains("soporte@olimpiait.com", enlaceCorreo.TextContent);
        }

        /// <summary>
        /// Verifica que al hacer clic en "Cerrar" se invoque el callback OnCerrar.
        /// </summary>
        [Fact]
        public void MostrarModal3_ClickCerrar_InvocaCallbackOnCerrar()
        {
            // Arrange
            var cerrarInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
            .Add(p => p.MostrarModal, 3)
          .Add(p => p.OnCerrar, EventCallback.Factory.Create(this, () => cerrarInvocado = true)));

            // Act
            var botonCerrar = cut.FindAll("button").First(b => b.TextContent.Contains("Cerrar") && !b.ClassList.Contains("btn-circle"));
            botonCerrar.Click();

            // Assert
            Assert.True(cerrarInvocado);
        }

        #endregion MostrarModal = 3 - Modal de Error

        #region MostrarModal = 4 - Modal de Anulación Parcial

        /// <summary>
        /// Verifica que el modal de anulación parcial muestre los totales correctos.
        /// </summary>
        [Fact]
        public void MostrarModal4_MuestraTotalesCorrectos()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
        .Add(p => p.MostrarModal, 4)
             .Add(p => p.Total, 10)
     .Add(p => p.TotalExitosos, 7));

            // Act & Assert
            var titulo = cut.Find("h3");
            Assert.Contains("Anulación parcial", titulo.TextContent);

            var mensajes = cut.FindAll("p");
            var mensajePrincipal = mensajes.First(p => p.TextContent.Contains("Se anularon correctamente"));

            Assert.Contains("7 de 10", mensajePrincipal.TextContent);
        }

        /// <summary>
        /// Verifica que el modal de anulación parcial muestre el ícono de advertencia.
        /// </summary>
        [Fact]
        public void MostrarModal4_MuestraIconoAdvertencia()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
                 .Add(p => p.MostrarModal, 4)
                         .Add(p => p.Total, 10)
                .Add(p => p.TotalExitosos, 7));

            // Act & Assert
            var iconoWarning = cut.Find(".bg-warning\\/20");
            Assert.NotNull(iconoWarning);
        }

        /// <summary>
        /// Verifica que el modal de anulación parcial muestre el correo de soporte.
        /// </summary>
        [Fact]
        public void MostrarModal4_MuestraCorreoSoporte()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
            .Add(p => p.MostrarModal, 4)
                      .Add(p => p.Total, 10)
            .Add(p => p.TotalExitosos, 7));

            // Act & Assert
            var enlaceCorreo = cut.Find("a[href^='mailto:']");
            Assert.Contains("soporte@olimpiait.com", enlaceCorreo.TextContent);
        }

        /// <summary>
        /// Verifica que el modal muestre el mensaje de verificación del listado.
        /// </summary>
        [Fact]
        public void MostrarModal4_MuestraMensajeVerificacionListado()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
       .Add(p => p.MostrarModal, 4)
 .Add(p => p.Total, 10)
      .Add(p => p.TotalExitosos, 7));

            // Act & Assert
            var mensajes = cut.FindAll("p");
            Assert.Contains(mensajes, p => p.TextContent.Contains("verifica el estado en el listado"));
        }

        /// <summary>
        /// Verifica que al hacer clic en "Entendido" se invoque el callback OnCerrar.
        /// </summary>
        [Fact]
        public void MostrarModal4_ClickEntendido_InvocaCallbackOnCerrar()
        {
            // Arrange
            var cerrarInvocado = false;
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
       .Add(p => p.MostrarModal, 4)
             .Add(p => p.Total, 10)
        .Add(p => p.TotalExitosos, 7)
        .Add(p => p.OnCerrar, EventCallback.Factory.Create(this, () => cerrarInvocado = true)));

            // Act
            var botonEntendido = cut.FindAll("button").First(b => b.TextContent.Contains("Entendido"));
            botonEntendido.Click();

            // Assert
            Assert.True(cerrarInvocado);
        }

        #endregion MostrarModal = 4 - Modal de Anulación Parcial

        #region Casos Especiales

        /// <summary>
        /// Verifica que no se renderice ningún modal cuando MostrarModal = 0.
        /// </summary>
        [Fact]
        public void MostrarModal0_NoMuestraNingunModal()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
            .Add(p => p.MostrarModal, 0));

            // Act & Assert
            var modales = cut.FindAll(".modal.modal-open");
            Assert.Empty(modales);
        }

        /// <summary>
        /// Verifica que no se renderice ningún modal cuando MostrarModal tiene un valor no válido.
        /// </summary>
        [Fact]
        public void MostrarModalInvalido_NoMuestraNingunModal()
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
       .Add(p => p.MostrarModal, 99));

            // Act & Assert
            var modales = cut.FindAll(".modal.modal-open");
            Assert.Empty(modales);
        }

        /// <summary>
        /// Verifica que cada modal tenga la clase "modal-open" cuando está visible.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void MostrarModal_ConValorValido_TieneClaseModalOpen(int valorModal)
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
     .Add(p => p.MostrarModal, valorModal)
     .Add(p => p.Total, 5)
 .Add(p => p.TotalExitosos, 3));

            // Act & Assert
            var modal = cut.Find(".modal.modal-open");
            Assert.NotNull(modal);
        }

        /// <summary>
        /// Verifica que todos los modales tengan el botón X para cerrar.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void MostrarModal_TodosTienenBotonXCerrar(int valorModal)
        {
            // Arrange
            var cut = RenderComponent<ModalFacturacion>(parameters => parameters
           .Add(p => p.MostrarModal, valorModal)
                      .Add(p => p.Total, 5)
                      .Add(p => p.TotalExitosos, 3));

            // Act & Assert
            var botonX = cut.Find("button.btn-circle");
            Assert.Contains("✕", botonX.TextContent);
        }

        #endregion Casos Especiales
    }
}

// Fin código generado por GitHub Copilot