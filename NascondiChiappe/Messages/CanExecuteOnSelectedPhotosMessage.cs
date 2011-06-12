namespace NascondiChiappe.Messages
{
    public class CanExecuteOnSelectedPhotosMessage
    {
        public bool CanExecute { get; set; }
        public CanExecuteOnSelectedPhotosMessage(bool canExecute)
        {
            CanExecute = canExecute;
        }
    }
}
