using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class FunctionService
        : IFunctionService
    {
        private readonly IFilePreProcessorService _filePreProcessorService;
        private readonly ISqlSriptExecutorService _sqlSriptExecutorService;

        public FunctionService(
            IFilePreProcessorService filePreProcessorService, 
            ISqlSriptExecutorService sqlSriptExecutorService)
        {
            _filePreProcessorService = filePreProcessorService ?? throw new ArgumentNullException(nameof(filePreProcessorService));
            _sqlSriptExecutorService = sqlSriptExecutorService ?? throw new ArgumentNullException(nameof(sqlSriptExecutorService));
        }

        public async Task<bool> ExecuteAsync(FunctionRequest request)
        {
            var preProcessResponse = await _filePreProcessorService.PreProcessAsync(request.FilePreProcessorData);
            var executeResponse = await _sqlSriptExecutorService.ExecuteAsync(request.Scripts);
            
            return preProcessResponse && executeResponse;
        }
    }
}
