using System.Collections.Generic;

namespace portalAdministrativoSISEC.Util.Const.Facturacion
{
    public static class FacturacionConst
    {
        #region Fields

        internal static readonly Dictionary<string, string> DescripcionArticulosFe = new()
        {
            { "Impuesto ANSV", "803"},
            { "Tarifa del SICOV", "804"},
            { "Tarifa del aliado de recaudo", "805"},
            { "Curso de conducción", "801"},
            { "A1", "701"},
            { "A2", "702"},
            { "B1", "703"},
            { "B2", "704"},
            { "B3", "705"},
            { "C1", "706"},
            { "C2", "707"},
            { "C3", "708"},
            { "RC1", "709"},
            { "Curso de instructor en conducción", "802"},
            { "IA1", "710"},
            { "IA2", "711"},
            { "IB1", "712"},
            { "IB2", "713"},
            { "IB3", "714"},
            { "IC1", "715"},
            { "IC2", "716"},
            { "IC3", "717"},
            { "Examen médico", "801"},
            { "Examen médico Sencillo", "701"},
            { "Examen médico Combo","702"}
        };

        #endregion Fields

        #region Regex

        public const string USUARIO_REGEX = @"^[A-Za-z0-9]{9,15}$";
        public const string CLAVE_REGEX = @"^(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,50}$";

        #endregion Regex
    }
}