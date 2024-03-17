
public interface IServiceProvider 
{
	TService GetService<TService>() where TService : class;
}
