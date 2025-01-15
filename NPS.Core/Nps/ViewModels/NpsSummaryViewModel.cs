namespace NPS.Core.Nps.ViewModels;

// Relatório com as contagens de cada categoria e o cálculo do NPS
public record NpsSummaryViewModel(decimal Promoters, decimal Neutrals, decimal Detractors, decimal NpsScore);
