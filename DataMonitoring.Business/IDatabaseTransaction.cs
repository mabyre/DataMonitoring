//
// https://docs.microsoft.com/fr-fr/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core
//

namespace DataMonitoring.Business
{
    public interface IDatabaseTransaction : System.IDisposable
    {
        void Commit();
        void Rollback();
    }
}