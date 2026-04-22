using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class SelectSuperTransporteDTO
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string codigo { get; set; }
        public string codigoDep { get; set; }

        private void setId(string value) { 
            id = value;
        }
        private void setNombre(string value)
        {
            nombre = value;
        }

        private void setAdicional(string value) {
            codigoDep = value;
        }

        private void setCodigo(string value)
        {
            codigo = value;
        }

        public static List<SelectSuperTransporteDTO> convertirSelectSuperTransporteDTO(List<object> lista, List<SelectSuperTransporteIndex> campos) {
            try
            {
                var resultado = new List<SelectSuperTransporteDTO>();
                foreach (var dato in lista)
                {
                    Type t = dato.GetType();
                    PropertyInfo[] props = t.GetProperties();
                    var row = new List<object>();
                    var select = new SelectSuperTransporteDTO();
                    foreach (var prop in props)
                    {
                        var campo = campos.Find(f => f.campoOriginal == prop.Name);
                        if (campo != null) {
                            var valorDato = prop.GetValue(dato);
                            if (valorDato != null)
                            {
                                if (campo.campoSelectSuperTransporte.ToUpper() == "ID")
                                    select.setId(valorDato.ToString());
                                if (campo.campoSelectSuperTransporte.ToUpper() == "NOMBRE")
                                    select.setNombre(valorDato.ToString());
                                if (campo.campoSelectSuperTransporte.ToUpper() == "CODIGO")
                                    select.setCodigo(valorDato.ToString());
                                if (campo.campoSelectSuperTransporte.ToUpper() == "CODIGODEP" && valorDato != null)
                                    select.setAdicional(valorDato.ToString());
                            }
                        }
                    }
                    resultado.Add(select);
                }
                return resultado;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }

    public class SelectSuperTransporteIndex {
        public string campoOriginal { get; set; }
        public string campoSelectSuperTransporte { get; set; }

        public static List<SelectSuperTransporteIndex> llenarCamposgenericos() {
            return new List<SelectSuperTransporteIndex>() {
                new SelectSuperTransporteIndex (){
                    campoOriginal = "id",
                    campoSelectSuperTransporte = "ID"
                },
                new SelectSuperTransporteIndex (){
                    campoOriginal = "nombre",
                    campoSelectSuperTransporte = "NOMBRE"
                }
            };
        }
        public static List<SelectSuperTransporteIndex> llenarCamposConCiudades()
        {
            return new List<SelectSuperTransporteIndex>() {
                new SelectSuperTransporteIndex (){
                    campoOriginal = "id",
                    campoSelectSuperTransporte = "ID"
                },
                new SelectSuperTransporteIndex (){
                    campoOriginal = "nombre",
                    campoSelectSuperTransporte = "NOMBRE"
                },
                new SelectSuperTransporteIndex (){
                    campoOriginal = "codigoDep",
                    campoSelectSuperTransporte = "CODIGODEP"
                },
                new SelectSuperTransporteIndex (){
                    campoOriginal = "codigo",
                    campoSelectSuperTransporte = "CODIGO"
                }
            };
        }
    }

    public class SelectSuperTransporteFiltro
    {
        public string campoOriginal { get; set; }
        public string valor { get; set; }
    }

    public class DepartamentosSuperTransporteDTO
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string codigoDep { get; set; }
        public string codigo { get; set; }
        public List<CiudadesSuperTransporteDTO> ciudades{ get; set; }
    }

    public class CiudadesSuperTransporteDTO
    {
        public string id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string codigoDep { get; set; }
    }
}
