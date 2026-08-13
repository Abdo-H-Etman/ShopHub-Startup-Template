using AutoMapper;
using myshop.BLL.DTOs.Category;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryListDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryListDto>>(categories);
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
    }

    public async Task UpdateAsync(UpdateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _unitOfWork.Categories.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
