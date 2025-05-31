using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebContracts.Data;
using WebContracts.Models;

namespace WebContracts.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        // Injeta o contexto do banco de dados
        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        // Exibe a tela de importação de contratos
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpGet] // Metódo responsável por trazer as consultas de contratos
        public async Task<IActionResult> Index()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Import)
                .OrderByDescending(c => c.con_dueDate)
                .ToListAsync();

            return View(contracts);
        }

        // Processa o upload e a leitura do CSV de contratos
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile csvFile)
        {
            // Verifica se o arquivo foi enviado
            if (csvFile == null || csvFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a CSV file.");
                return View();
            }

            //  Simula um usuário logado (por enquanto está fixo, mas o ideal é pegar do contexto de autenticação)
            int currentUserId = 1;

            //  Cria um novo registro na tabela Imports para registrar essa importação
            var import = new Import
            {
                im_fileName = csvFile.FileName,
                im_dateImport = DateTime.Now,
                user_id = currentUserId
            };

            // Salva no banco para gerar o im_id (chave estrangeira dos contratos)
            _context.Imports.Add(import);
            await _context.SaveChangesAsync();

            var contracts = new List<Contract>();

            // Configura o CsvHelper para ler arquivos com delimitador ";"
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null, // Ignora validação de cabeçalho
                MissingFieldFound = null // Ignora campos faltantes
            };

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, config))
            {
                try
                {
                    // Lê todos os registros do CSV e mapeia para o DTO ContractCsv
                    var records = csv.GetRecords<ContractCsv>().ToList();

                    foreach (var record in records)
                    {
                        // Cria um novo contrato e associa ao ID da importação
                        var contract = new Contract
                        {
                            im_id = import.im_id,
                            con_customerName = record.customerName,
                            con_cpf = record.cpf,
                            con_contractNumber = record.contractNumber,
                            con_product = record.product,
                            con_dueDate = (DateTime)record.dueDate,
                            con_amount = record.amount 
                        };

                        contracts.Add(contract);
                    }

                    // Salva todos os contratos de uma vez só
                    _context.Contracts.AddRange(contracts);
                    await _context.SaveChangesAsync();

                    // Mensagem de sucesso que será exibida na próxima tela
                    TempData["Success"] = $"{contracts.Count} contracts imported successfully!";
                    return RedirectToAction("Index", "Imports");
                }
                catch (Exception ex)
                {
                    // Se der erro, exibe a mensagem no formulário
                    ModelState.AddModelError("", $"Error importing CSV: {ex.Message}");
                }
            }

            // Se algo deu errado, retorna para a mesma view com os erros
            return View();
        }
    }
}
