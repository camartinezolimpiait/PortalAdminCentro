namespace portalAdministrativoSISEC.Entidades.SuperTransporte
{
    public class LoaderEventSubmit
    {
        private bool showLoader { get; set; } = false;

        public void Show() { 
            showLoader = true;
        }

        public void Hide()
        {
            showLoader = false;
        }

        public bool IsVisible() { 
            return showLoader;
        }
    }
}
