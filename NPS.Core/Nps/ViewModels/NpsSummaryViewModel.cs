namespace NPS.Core.Nps.ViewModels;

// Relatório com as contagens de cada categoria e o cálculo do NPS
public record NpsSummaryViewModel(int Promoters, int Neutrals, int Detractors, int NpsScore);
