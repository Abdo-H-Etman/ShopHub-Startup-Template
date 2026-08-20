using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using myshop.BLL.DTOs.Category;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class CategoryService : ICategoryService
{
    private const string CategoriesCacheKey = "categories";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoryListDto>> GetAllAsync()
    {
        if (_cache.TryGetValue(
            CategoriesCacheKey,
            out IEnumerable<CategoryListDto>? cachedCategories))
        {
            return cachedCategories!;
        }
        var categories = await _unitOfWork.Categories.GetAllAsync();

        var categoryDtos =
        _mapper.Map<IEnumerable<CategoryListDto>>(categories);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        _cache.Set(
            CategoriesCacheKey,
            categoryDtos,
            cacheOptions);

        return categoryDtos;
    }

    public async Task<CategoryListDto?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category is null ? null : _mapper.Map<CategoryListDto>(category);
    }

    public async Task<UpdateCategoryDto?> GetByIdForUpdateAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category is null ? null : _mapper.Map<UpdateCategoryDto>(category);
    }

    public async Task CreateAsync(CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove(CategoriesCacheKey);
    }

    public async Task UpdateAsync(UpdateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove(CategoriesCacheKey);
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Categories.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove(CategoriesCacheKey);
    }
}
