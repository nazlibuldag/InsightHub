using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightHub.Application.Interfaces;

public interface IStatisticsService
{
    double GetMin(List<double> values);

    double GetMax(List<double> values);

    double GetAverage(List<double> values);

    double GetMedian(List<double> values);

    double GetStandardDeviation(List<double> values);
}