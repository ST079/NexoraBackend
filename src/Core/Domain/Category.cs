using NexoraBackend.Core.Exceptions;
using NexoraBackend.Core.ValueObjects;

namespace NexoraBackend.Core.Domain;

/// Category supports a self-referencing tree (parent/children) for nested
/// taxonomies like: Electronics → Phones → Smartphones.
/// ParentId = null means it is a root category.
/// We keep it as a full entity (not value object) because categories
/// have identity, lifecycle, and can be independently created/edited.
/// </summary>
public class Category : Auditable
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public string? ImageUrl { get; private set; }

    //Instead of deleting category:, you deactivate it. Because:
    //soft business deletion > hard DB deletion
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }

    // Self-referencing hierarchy
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }

    private readonly List<Category> _children = [];
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { }


    public static Category Create(string name, string description,
        Guid? parentId = null, string? imageUrl = null, int sortOrder = 0)
    {
        return new Category
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Slug = Slug.Create(name),
            ParentId = parentId,
            ImageUrl = imageUrl,
            SortOrder = sortOrder
        };
    }

    public void Update(string name, string description, string? imageUrl, int sortOrder)
    {
        Name = name.Trim();
        Description = description.Trim();
        Slug = Slug.Create(name);
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public bool IsRoot => ParentId is null;
    public bool HasChildren => _children.Count > 0;
}