using Bunit;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using portalAdministrativoSISEC.Pages.Facturacion.Estado;
using portalAdministrativoSISEC.Services.MiLicencia;
using portalAdministrativoSISEC.Entidades.Facturacion;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace TestUnitPortalAdministrativoSisec.Facturacion
{
    /// <summary>
    /// Pruebas unitarias para el componente Estado.razor
    /// </summary>
    public class EstadoTests : TestContext
    {
        private readonly Mock<IMiLicenciaService> _mockMiLicenciaService;

        public EstadoTests()
        {
            _mockMiLicenciaService = new Mock<IMiLicenciaService>();
            Services.AddSingleton(_mockMiLicenciaService.Object);

            // Agregar el EditContext requerido por DataAnnotationsValidator
            ComponentFactories.AddStub<DataAnnotationsValidator>();
        }

        #region Escenario 1: ErrorFacturacion es null

        [Fact(DisplayName = "Debe mostrar mensaje cuando ErrorFacturacion es null")]
        public void DeberMostrarMensajeCuandoErrorFacturacionEsNull()
        {
            // Arrange
            _mockMiLicenciaService
       .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
           .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
    {
  SolicitudExitosa = true,
        Datos = null
       });

            // Act
      var cut = RenderComponent<Estado>(parameters => parameters
    .Add(p => p.IdRunt, "123456")
   .Add(p => p.Proveedor, false));

       // Assert
  cut.WaitForAssertion(() =>
        {
    var statusCard = cut.Find(".status-card");
  Assert.NotNull(statusCard);
        Assert.Contains("No hay información de facturación disponible", statusCard.TextContent);
      });
      }

        [Fact(DisplayName = "No debe mostrar status-header cuando ErrorFacturacion es null")]
      public void NoDebeMostrarStatusHeaderCuandoErrorFacturacionEsNull()
        {
            // Arrange
            _mockMiLicenciaService
    .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
          .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
  {
  SolicitudExitosa = true,
Datos = null
     });

       // Act
            var cut = RenderComponent<Estado>(parameters => parameters
            .Add(p => p.IdRunt, "123456")
  .Add(p => p.Proveedor, false));

            // Assert
        cut.WaitForAssertion(() =>
         {
    var statusHeaders = cut.FindAll(".status-header");
         Assert.Empty(statusHeaders);
    });
        }

    #endregion

#region Escenario 2: ErrorFacturacion tiene datos

        [Fact(DisplayName = "Debe renderizar correctamente cuando ErrorFacturacion tiene datos")]
        public void DebeRenderizarCorrectamenteCuandoErrorFacturacionTieneDatos()
        {
        // Arrange
     var respuestaFacturacion = new RespuestaErrorFacturacion
   {
    Estado = "En línea",
Proveedor = "Proveedor Test",
       ProximaFactura = "001",
           UltimaActualizacion = "2024-01-15 10:30:00",
                DetalleErrores = new List<DetalleErrorFacturacion>()
 };

            _mockMiLicenciaService
                .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
  .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
        {
   SolicitudExitosa = true,
        Datos = respuestaFacturacion
      });

            // Act
        var cut = RenderComponent<Estado>(parameters => parameters
                .Add(p => p.IdRunt, "123456")
        .Add(p => p.Proveedor, true));

// Assert
  cut.WaitForAssertion(() =>
            {
 Assert.Contains("En línea", cut.Markup);
                Assert.Contains("Proveedor Test", cut.Markup);
      Assert.Contains("001", cut.Markup);
     Assert.Contains("2024-01-15 10:30:00", cut.Markup);
  });
        }

        [Fact(DisplayName = "Debe aplicar clase CSS correcta según el estado")]
        public void DebeAplicarClaseCssCorrectaSegunElEstado()
        {
         // Arrange
         var respuestaFacturacion = new RespuestaErrorFacturacion
            {
        Estado = "Error",
    Proveedor = "Proveedor Test",
     ProximaFactura = "001",
        UltimaActualizacion = "2024-01-15 10:30:00",
     DetalleErrores = new List<DetalleErrorFacturacion>()
          };

   _mockMiLicenciaService
    .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
      .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
            {
         SolicitudExitosa = true,
        Datos = respuestaFacturacion
           });

  // Act
            var cut = RenderComponent<Estado>(parameters => parameters
     .Add(p => p.IdRunt, "123456")
              .Add(p => p.Proveedor, false));

     // Assert
       cut.WaitForAssertion(() =>
     {
    var statusDot = cut.Find(".status-dot");
       Assert.Contains("error-estado", statusDot.ClassName);
            });
     }

        [Fact(DisplayName = "No debe mostrar proveedor cuando el parámetro Proveedor es false")]
        public void NoDebeMostrarProveedorCuandoParametroProveedorEsFalse()
        {
     // Arrange
        var respuestaFacturacion = new RespuestaErrorFacturacion
    {
              Estado = "En línea",
             Proveedor = "Proveedor Test",
                ProximaFactura = "001",
  UltimaActualizacion = "2024-01-15 10:30:00",
    DetalleErrores = new List<DetalleErrorFacturacion>()
 };

          _mockMiLicenciaService
        .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
     .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
  {
        SolicitudExitosa = true,
        Datos = respuestaFacturacion
     });

      // Act
  var cut = RenderComponent<Estado>(parameters => parameters
     .Add(p => p.IdRunt, "123456")
      .Add(p => p.Proveedor, false));

    // Assert
            cut.WaitForAssertion(() =>
      {
    Assert.DoesNotContain("Proveedor:", cut.Markup);
         });
   }

  #endregion

        #region Escenario 3: Renderizado de DetalleErrores

   [Fact(DisplayName = "Debe renderizar DetalleErrores cuando existen errores")]
        public void DebeRenderizarDetalleErroresCuandoExistenErrores()
        {
            // Arrange
  var respuestaFacturacion = new RespuestaErrorFacturacion
    {
      Estado = "Error",
              Proveedor = "Proveedor Test",
              ProximaFactura = "001",
          UltimaActualizacion = "2024-01-15 10:30:00",
             DetalleErrores = new List<DetalleErrorFacturacion>
         {
         new DetalleErrorFacturacion
{
            Error = "Error de conexión",
      FechaEstado = "2024-01-15 10:00:00",
  DescripcionError = "No se pudo conectar con el servicio de facturación"
           },
        new DetalleErrorFacturacion
     {
    Error = "Timeout",
  FechaEstado = "2024-01-15 09:30:00",
  DescripcionError = "La solicitud excedió el tiempo de espera"
     }
    }
 };

         _mockMiLicenciaService
 .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
                .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
     {
   SolicitudExitosa = true,
            Datos = respuestaFacturacion
      });

        // Act
        var cut = RenderComponent<Estado>(parameters => parameters
      .Add(p => p.IdRunt, "123456")
    .Add(p => p.Proveedor, false));

       // Assert
    cut.WaitForAssertion(() =>
            {
          var statusBody = cut.Find(".status-body");
        Assert.NotNull(statusBody);
        Assert.Contains("Error de conexión", cut.Markup);
      Assert.Contains("No se pudo conectar con el servicio de facturación", cut.Markup);
      Assert.Contains("Timeout", cut.Markup);
        Assert.Contains("La solicitud excedió el tiempo de espera", cut.Markup);
            });
        }

  [Fact(DisplayName = "No debe renderizar status-body cuando no hay errores")]
        public void NoDebeRenderizarStatusBodyCuandoNoHayErrores()
        {
            // Arrange
            var respuestaFacturacion = new RespuestaErrorFacturacion
 {
                Estado = "En línea",
           Proveedor = "Proveedor Test",
            ProximaFactura = "001",
     UltimaActualizacion = "2024-01-15 10:30:00",
          DetalleErrores = new List<DetalleErrorFacturacion>()
          };

  _mockMiLicenciaService
    .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
        .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
         {
         SolicitudExitosa = true,
    Datos = respuestaFacturacion
       });

         // Act
   var cut = RenderComponent<Estado>(parameters => parameters
            .Add(p => p.IdRunt, "123456")
     .Add(p => p.Proveedor, false));

            // Assert
  cut.WaitForAssertion(() =>
            {
    var statusBodies = cut.FindAll(".status-body");
             Assert.Empty(statusBodies);
            });
        }

     [Fact(DisplayName = "Debe renderizar múltiples mensajes de error correctamente")]
        public void DebeRenderizarMultiplesMensajesDeErrorCorrectamente()
        {
       // Arrange
            var respuestaFacturacion = new RespuestaErrorFacturacion
            {
     Estado = "Error",
      Proveedor = "Proveedor Test",
     ProximaFactura = "001",
                UltimaActualizacion = "2024-01-15 10:30:00",
     DetalleErrores = new List<DetalleErrorFacturacion>
   {
          new DetalleErrorFacturacion
          {
           Error = "Error 1",
    FechaEstado = "2024-01-15 10:00:00",
  DescripcionError = "Descripción del error 1"
           },
       new DetalleErrorFacturacion
        {
   Error = "Error 2",
               FechaEstado = "2024-01-15 09:30:00",
  DescripcionError = "Descripción del error 2"
               },
                new DetalleErrorFacturacion
    {
       Error = "Error 3",
            FechaEstado = "2024-01-15 09:00:00",
          DescripcionError = "Descripción del error 3"
          }
      }
 };

       _mockMiLicenciaService
              .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
       .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
          {
   SolicitudExitosa = true,
  Datos = respuestaFacturacion
   });

  // Act
  var cut = RenderComponent<Estado>(parameters => parameters
      .Add(p => p.IdRunt, "123456")
        .Add(p => p.Proveedor, false));

            // Assert
            cut.WaitForAssertion(() =>
    {
          var statusMessages = cut.FindAll(".status-message");
                Assert.Equal(6, statusMessages.Count); // 2 mensajes por cada error (título + descripción)
   });
  }

     #endregion

        #region Escenario 4: El botón Actualizar estado

   [Fact(DisplayName = "Debe existir el botón de actualizar estado")]
        public void DebeExistirElBotonDeActualizarEstado()
        {
   // Arrange
         var respuestaFacturacion = new RespuestaErrorFacturacion
    {
            Estado = "En línea",
    Proveedor = "Proveedor Test",
        ProximaFactura = "001",
       UltimaActualizacion = "2024-01-15 10:30:00",
                DetalleErrores = new List<DetalleErrorFacturacion>()
            };

            _mockMiLicenciaService
       .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
     .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
    {
                    SolicitudExitosa = true,
               Datos = respuestaFacturacion
         });

     // Act
            var cut = RenderComponent<Estado>(parameters => parameters
            .Add(p => p.IdRunt, "123456")
       .Add(p => p.Proveedor, false));

        // Assert
        cut.WaitForAssertion(() =>
       {
   var refreshButton = cut.Find(".refresh-btn");
         Assert.NotNull(refreshButton);
    });
        }

    [Fact(DisplayName = "Debe llamar al servicio al hacer clic en actualizar")]
        public async Task DebeLlamarAlServicioAlHacerClicEnActualizar()
        {
      // Arrange
       var respuestaFacturacion = new RespuestaErrorFacturacion
            {
        Estado = "En línea",
    Proveedor = "Proveedor Test",
       ProximaFactura = "001",
     UltimaActualizacion = "2024-01-15 10:30:00",
  DetalleErrores = new List<DetalleErrorFacturacion>()
  };

            _mockMiLicenciaService
     .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
     .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
            {
        SolicitudExitosa = true,
 Datos = respuestaFacturacion
        });

            var cut = RenderComponent<Estado>(parameters => parameters
       .Add(p => p.IdRunt, "123456")
    .Add(p => p.Proveedor, false));

    cut.WaitForAssertion(() => cut.Find(".refresh-btn"));

      // Act
            var refreshButton = cut.Find(".refresh-btn");
         await cut.InvokeAsync(() => refreshButton.Click());

  // Assert
      _mockMiLicenciaService.Verify(
      s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()),
    Times.AtLeast(2)); // Una vez en OnInitializedAsync y otra al hacer clic
        }

        [Fact(DisplayName = "Debe mostrar tooltip en el botón de actualizar")]
        public void DebeMostrarTooltipEnElBotonDeActualizar()
        {
   // Arrange
         var respuestaFacturacion = new RespuestaErrorFacturacion
 {
     Estado = "En línea",
            Proveedor = "Proveedor Test",
        ProximaFactura = "001",
   UltimaActualizacion = "2024-01-15 10:30:00",
        DetalleErrores = new List<DetalleErrorFacturacion>()
          };

 _mockMiLicenciaService
       .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
         .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
     {
  SolicitudExitosa = true,
Datos = respuestaFacturacion
        });

    // Act
            var cut = RenderComponent<Estado>(parameters => parameters
       .Add(p => p.IdRunt, "123456")
                .Add(p => p.Proveedor, false));

   // Assert
        cut.WaitForAssertion(() =>
        {
    var tooltip = cut.Find(".tooltip");
             Assert.NotNull(tooltip);
       Assert.Contains("Actualizar estado", tooltip.TextContent);
   });
        }

        #endregion

        #region Escenario 5: Botón deshabilitado cuando IsLoading es true

        [Fact(DisplayName = "El botón debe estar deshabilitado cuando IsLoading es true")]
        public void ElBotonDebeEstarDeshabilitadoCuandoIsLoadingEsTrue()
        {
          // Arrange
       var tcs = new TaskCompletionSource<FacturacionResponse<RespuestaErrorFacturacion>>();
    
            _mockMiLicenciaService
    .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
          .Returns(tcs.Task);

    // Act
          var cut = RenderComponent<Estado>(parameters => parameters
        .Add(p => p.IdRunt, "123456")
       .Add(p => p.Proveedor, false));

    // Assert - Durante la carga, el botón no existe porque el contenido está cargando
            // El SpinLoader está mostrando el LoadingTemplate
   var loadingDiv = cut.Find("div[style*='height:100%']");
 Assert.NotNull(loadingDiv);

            // Complete la tarea para que se muestre el contenido
            tcs.SetResult(new FacturacionResponse<RespuestaErrorFacturacion>
   {
     SolicitudExitosa = true,
          Datos = new RespuestaErrorFacturacion
              {
         Estado = "En línea",
              Proveedor = "Proveedor Test",
   ProximaFactura = "001",
     UltimaActualizacion = "2024-01-15 10:30:00",
   DetalleErrores = new List<DetalleErrorFacturacion>()
          }
 });

            // Assert - Después de la carga, el botón existe y no está deshabilitado
            cut.WaitForAssertion(() =>
   {
       var button = cut.Find(".refresh-btn");
       Assert.False(button.HasAttribute("disabled"));
    });
        }

        [Fact(DisplayName = "El spinner debe mostrarse cuando IsLoading es true")]
 public void ElSpinnerDebeMostarseCuandoIsLoadingEsTrue()
        {
   // Arrange
            var tcs = new TaskCompletionSource<FacturacionResponse<RespuestaErrorFacturacion>>();
       
            _mockMiLicenciaService
          .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
            .Returns(tcs.Task);

   // Act
            var cut = RenderComponent<Estado>(parameters => parameters
       .Add(p => p.IdRunt, "123456")
              .Add(p => p.Proveedor, false));

  // Assert - El loading template debe estar visible (verifica el div contenedor del loading)
  var loadingDiv = cut.Find("div[style*='height:100%']");
            Assert.NotNull(loadingDiv);
            Assert.Contains("display:flex", loadingDiv.GetAttribute("style"));
        }

      [Fact(DisplayName = "El contenido debe mostrarse cuando IsLoading es false")]
        public void ElContenidoDebeMostarseCuandoIsLoadingEsFalse()
   {
            // Arrange
       var respuestaFacturacion = new RespuestaErrorFacturacion
          {
                Estado = "En línea",
  Proveedor = "Proveedor Test",
   ProximaFactura = "001",
     UltimaActualizacion = "2024-01-15 10:30:00",
   DetalleErrores = new List<DetalleErrorFacturacion>()
          };

   _mockMiLicenciaService
        .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
    .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
 {
    SolicitudExitosa = true,
                 Datos = respuestaFacturacion
              });

        // Act
            var cut = RenderComponent<Estado>(parameters => parameters
    .Add(p => p.IdRunt, "123456")
      .Add(p => p.Proveedor, false));

          // Assert
            cut.WaitForAssertion(() =>
            {
  var statusCard = cut.Find(".status-card");
          Assert.NotNull(statusCard);
            Assert.Contains("En línea", cut.Markup);
        });
        }

    #endregion

        #region Pruebas adicionales de GetClassEstado

   [Theory(DisplayName = "Debe aplicar la clase CSS correcta según diferentes estados")]
        [InlineData("Error", "error-estado")]
 [InlineData("Sin conexión", "sin-conexion-estado")]
        [InlineData("Apagado", "apagado-estado")]
        [InlineData("En línea", "linea-estado")]
    [InlineData("Desconocido", "apagado-estado")] // Estado por defecto
        public void DebeAplicarLaClaseCssCorrectaSegunDiferentesEstados(string estado, string claseEsperada)
        {
  // Arrange
      var respuestaFacturacion = new RespuestaErrorFacturacion
  {
  Estado = estado,
    Proveedor = "Proveedor Test",
       ProximaFactura = "001",
        UltimaActualizacion = "2024-01-15 10:30:00",
        DetalleErrores = new List<DetalleErrorFacturacion>()
  };

_mockMiLicenciaService
    .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
 .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
        {
      SolicitudExitosa = true,
                 Datos = respuestaFacturacion
          });

 // Act
          var cut = RenderComponent<Estado>(parameters => parameters
      .Add(p => p.IdRunt, "123456")
       .Add(p => p.Proveedor, false));

          // Assert
      cut.WaitForAssertion(() =>
{
     var statusDot = cut.Find(".status-dot");
        Assert.Contains(claseEsperada, statusDot.ClassName);
            });
        }

  #endregion

        #region Pruebas de manejo de errores

        [Fact(DisplayName = "Debe manejar error cuando el servicio falla")]
        public void DebeManejarErrorCuandoElServicioFalla()
      {
    // Arrange
            _mockMiLicenciaService
     .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
            .ThrowsAsync(new System.Exception("Error de conexión"));

          _mockMiLicenciaService
.Setup(s => s.ShowNotificacion(It.IsAny<portalAdministrativoSISEC.Enum.NotificationStatus>(), It.IsAny<string>()))
        .Returns(Task.CompletedTask);

 // Act
  var cut = RenderComponent<Estado>(parameters => parameters
    .Add(p => p.IdRunt, "123456")
    .Add(p => p.Proveedor, false));

  // Assert
       cut.WaitForAssertion(() =>
            {
   _mockMiLicenciaService.Verify(
          s => s.ShowNotificacion(
  It.IsAny<portalAdministrativoSISEC.Enum.NotificationStatus>(),
       It.IsAny<string>()),
           Times.AtLeastOnce);
   });
      }

   [Fact(DisplayName = "Debe mostrar mensaje cuando la solicitud no es exitosa")]
        public void DebeMostrarMensajeCuandoLaSolicitudNoEsExitosa()
  {
   // Arrange
            _mockMiLicenciaService
         .Setup(s => s.ConsultarEstadoSistema(It.IsAny<ConsultaEstadoFacturacion>()))
        .ReturnsAsync(new FacturacionResponse<RespuestaErrorFacturacion>
{
         SolicitudExitosa = false,
           Mensaje = "Error en el servicio",
     Datos = null
       });

            _mockMiLicenciaService
          .Setup(s => s.ShowNotificacion(It.IsAny<portalAdministrativoSISEC.Enum.NotificationStatus>(), It.IsAny<string>()))
    .Returns(Task.CompletedTask);

// Act
            var cut = RenderComponent<Estado>(parameters => parameters
     .Add(p => p.IdRunt, "123456")
        .Add(p => p.Proveedor, false));

        // Assert
      cut.WaitForAssertion(() =>
            {
   _mockMiLicenciaService.Verify(
       s => s.ShowNotificacion(
              portalAdministrativoSISEC.Enum.NotificationStatus.Error,
            "No se pudo obtener el estado del sistema."),
        Times.Once);
});
        }

        #endregion
 }
}
// Fin código generado por GitHub Copilot