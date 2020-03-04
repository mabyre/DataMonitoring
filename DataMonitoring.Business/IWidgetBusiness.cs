using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface IWidgetBusiness : IUnitOfWork
    {
        Task<Widget> GetWidgetAsync(long id);
        long CreateOrUpdateWidget(Widget widget);
        void DeleteWidget(long id);
        void DuplicateWidget(Widget widget, string copyMessage);
        string GetIndicatorTitleList(long id);
    }
}
