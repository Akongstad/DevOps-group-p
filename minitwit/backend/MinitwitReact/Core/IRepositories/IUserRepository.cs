namespace MinitwitReact.Core.IRepositories;

public interface IUserRepository
{
    
}

/*namespace VideoOverflow.Core.IRepositories;

/// <summary>
/// The interface for the resource repository. This ensures all the crud methods are implemented as well as a
/// GetResources method which gets all resources based on a query from the user
/// </summary>
public interface IResourceRepository
{
    public Task<IEnumerable<ResourceDetailsDTO>> GetAll();
    public Task<IEnumerable<ResourceDTO>> GetResources(int category, string query, IEnumerable<TagDTO> tags, int count, int skip);
    public Task<Option<ResourceDetailsDTO>> Get(int id);
    public Task<Status> Push(ResourceCreateDTO create);
    public Task<Status> Update(ResourceUpdateDTO update);

    public Task<Status> Delete(int resourceId);
}*/