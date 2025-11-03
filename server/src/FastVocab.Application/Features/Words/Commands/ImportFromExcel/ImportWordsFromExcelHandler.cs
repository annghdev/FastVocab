using AutoMapper;
using ClosedXML.Excel;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using MediatR;

namespace FastVocab.Application.Features.Words.Commands.ImportFromExcel;

public class ImportWordsFromExcelHandler : IRequestHandler<ImportWordsFromExcelCommand, ImportExcelResult<WordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ImportWordsFromExcelHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ImportExcelResult<WordDto>> Handle(ImportWordsFromExcelCommand request, CancellationToken cancellationToken)
    {
        ImportExcelResult<WordDto> importResult = new();

        using var stream = new MemoryStream();
        await request.File.CopyToAsync(stream);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var lastRow = worksheet.LastRowUsed().RowNumber();
        if (lastRow < 3)
        {
            importResult.Error = Error.None;
            return importResult;
        }

        string topicName = worksheet.Cell(1, 2).GetValue<string>();
        string topicVnText = worksheet.Cell(1, 4).GetValue<string>();

        var topic = await _unitOfWork.Topics.FindAsync(t => t.Name == topicName);
        if (topic == null)
        {
            topic = new Topic
            {
                Name = topicName,
                VnText = topicVnText
            };
            _unitOfWork.Topics.Add(topic);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        List<Word> words = [];

        for (int row = 3; row <= lastRow; row++) // bỏ qua header
        {
            var word = new Word
            {
                Text = worksheet.Cell(row, 2).GetValue<string>(),
                Type = worksheet.Cell(row, 3).GetValue<string>(),
                Meaning = worksheet.Cell(row, 4).GetValue<string>(),
                Definition = worksheet.Cell(row, 5).GetValue<string>(),
                Level = worksheet.Cell(row, 6).GetValue<string>(),
                Example1 = worksheet.Cell(row, 7).GetValue<string>(),
                Example2 = worksheet.Cell(row, 8).GetValue<string>(),
                Example3 = worksheet.Cell(row, 9).GetValue<string>(),
                Topics = []
            };
            words.Add(word);
        }

        List<Word> createdWords = [];
        foreach (var word in words)
        {
            var existWord = await _unitOfWork.Words.FindAsync(w => w.Text == word.Text);
            if (existWord == null)
            {
                word.Topics?.Add(new WordTopic
                {
                    Topic = topic,
                    Word = word
                });
                _unitOfWork.Words.Add(word);
                createdWords.Add(word);
            }
            else
            {
                importResult.ErrorDetails.Add(word.Text);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        importResult.Data.AddRange(_mapper.Map<IEnumerable<WordDto>>(createdWords));

        return importResult;
    }
}
