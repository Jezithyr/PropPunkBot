using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Core;

public static class EFCHelpers
{
    public static void FindAndUpdateData<T>(DbContext context, Func<T,bool> predicate,Func<T, T> updateData) where T : class
    {
        var record = context.Set<T>().AsNoTracking().First(predicate);
        var updatedData = updateData(record);
        context.Update(updatedData);
        context.SaveChanges();
    }

    public static void UpdateData<T>(DbContext context,ref T model, Func<T, T> updateData) where T : class
    {
        var updatedData = updateData(model);
        model = updatedData;
        context.Update(updatedData);
        context.SaveChanges();
    }
}
