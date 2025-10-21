using AutoMapper;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Shared.DTOs.WordLists;

namespace FastVocab.Application.Common.Mappings;

public class WordListProfile : Profile
{
    public WordListProfile()
    {
        CreateMap<WordList, WordListDto>();
        CreateMap<CreateWordListRequest, WordList>();
    }
}
