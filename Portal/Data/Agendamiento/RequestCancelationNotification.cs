namespace portalAdministrativoSISEC.Data.Agendamiento
{
    public class RequestNotificationCancelation
    {
        public string AplicantName { get; set; } // aceptar caracteres especiales, espacios, 
        public string StatusAppointment { get; set; }// agendada , cancelada 
        public string ShortDate { get; set; }//  8 de abril
        public string Hour { get; set; }// 9:30 am
        public string Day { get; set; }// Sabado 
        public string Year { get; set; }// 2025
        public string CenterName { get; set; }// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
        public string CenterAddress { get; set; }//// nombre debe aceptar espacios, un guion, # , acentos, numeros,  
        public string CenterPhone { get; set; }// // Solo numeros
        public string Email { get; set; } 
    }
}
