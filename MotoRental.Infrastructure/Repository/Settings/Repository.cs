using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MotoRental.Infrastructure.Help;
using System.Linq.Expressions;
using MotoRental.Infrastructure.Repository.Settings.Interface;

namespace MotoRental.Infrastructure.Repository.Settings;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected IDbConnection dbConn;
    private IDbTransaction dbTransaction;
    private bool isTransactionOwner = false;
    public Repository(IDbConnection databaseConnection)
    {
        dbConn = databaseConnection;
        if (dbConn.State != ConnectionState.Open)
            dbConn.Open();
    }


    protected Repository(IDatabaseFactory databaseOptions)
    {
        dbConn = databaseOptions.GetDbConnection;
    }

    protected IDbTransaction EnsureTransaction()
    {
        if (dbConn.State != ConnectionState.Open)
            dbConn.Open();

        if (dbTransaction == null)
        {
            dbTransaction = dbConn.BeginTransaction();
            isTransactionOwner = true;
        }
        return dbTransaction;
    }

    protected void CommitTransaction()
    {
        if (dbTransaction != null && isTransactionOwner)
        {
            dbTransaction.Commit();
            dbTransaction.Dispose();
            dbTransaction = null;
            isTransactionOwner = false;
        }
    }

    protected void RollbackTransaction()
    {
        if (dbTransaction != null && isTransactionOwner)
        {
            dbTransaction.Rollback();
            dbTransaction.Dispose();
            dbTransaction = null;
            isTransactionOwner = false;
        }
    }

    public void Dispose()
    {
        if (dbTransaction != null)
        {
            dbTransaction.Dispose();
        }

        if (dbConn != null)
        {
            dbConn.Close();
            dbConn.Dispose();
        }
    }

    public async Task SaveChangesAsync()
    {
        CommitTransaction();
    }

    public void Rollback()
    {
        RollbackTransaction();
    }


    public static string Schema { get; set; } = typeof(TEntity).GetCustomAttribute<TableAttribute>()?.Schema ?? "public";
    private string SelectByIdQuery { get; set; } = $"SELECT {{0}} FROM \"{Schema}\".\"{typeof(TEntity).Name}\" WHERE {{1}} {{2}}";
    private string SelectAllQuery { get; set; } = $"SELECT {{0}} FROM \"{Schema}\".\"{typeof(TEntity).Name}\" {{1}}";
    private string SelectValidateQuery { get; set; } = $"SELECT {{0}} FROM \"{Schema}\".\"{typeof(TEntity).Name}\" WHERE {{1}}";
    private string SelectCountQuery { get; set; } = $"SELECT COUNT(*) FROM \"{Schema}\".\"{typeof(TEntity).Name}\" WHERE {{0}}";

    private string InsertQuery { get; set; } = $"INSERT INTO \"{Schema}\".\"{typeof(TEntity).Name}\" ({{0}}) VALUES ({{1}})";
    private string InsertQueryReturnInserted { get; set; } = $"INSERT INTO \"{Schema}\".\"{typeof(TEntity).Name}\" ({{0}}) VALUES ({{1}}) RETURNING {{2}}";
    private string UpdateByIdQuery { get; set; } = $"UPDATE \"{Schema}\".\"{typeof(TEntity).Name}\" SET {{0}} WHERE {{1}}";
    private string DeleteByIdQuery { get; set; } = $"DELETE FROM \"{Schema}\".\"{typeof(TEntity).Name}\" WHERE {{0}}";



    public async Task AddAsync(TEntity entity, bool includeKey = false)
    {
        var insertClause = BuildInsertClause(entity, includeKey);
        string query = string.Format(InsertQuery, insertClause.Columns, insertClause.Values);
        await dbConn.ExecuteAsync(query, entity, transaction: EnsureTransaction());
    }

    public async Task<TIdentity> AddGetIdentityAsync<TIdentity>(TEntity entity, bool includeKey = false)
    {
        var insertClause = BuildInsertClause(entity, includeKey);
        string query = string.Format(InsertQueryReturnInserted, insertClause.Columns, insertClause.Values, $"\"{typeof(TEntity).GetKeyProperty().Name}\"");

        TIdentity insertedEntity = await dbConn.QuerySingleAsync<TIdentity>(query, entity);
        return insertedEntity;
    }

    public async Task<TEntity> AddGetEntityAsync(TEntity entity, bool includeKey = false)
    {
        var insertClause = BuildInsertClause(entity, includeKey);
        string selectedColumns = BuildSelectClause();

        string query = string.Format(InsertQueryReturnInserted, insertClause.Columns, insertClause.Values, selectedColumns);

        TEntity insertedEntity = await dbConn.QuerySingleAsync<TEntity>(query, entity, transaction: EnsureTransaction());
        return insertedEntity;
    }


    public async Task DeleteAsync(Expression<Func<TEntity, bool>> wherexpression)
    {
        var where = BuildWhereClause(wherexpression);
        string query = string.Format(DeleteByIdQuery, where.WhereClause);

        await dbConn.ExecuteAsync(query, where.Parameters, transaction: EnsureTransaction());
    }

    public async Task DeleteAsync(TEntity entity)
    {
        var where = BuildWhereClauseForKey(entity);
        string query = string.Format(DeleteByIdQuery, where);
        await dbConn.ExecuteAsync(query, entity, transaction: EnsureTransaction());
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, object>> orderBy = null, bool ascending = true)
    {
        var selectClause = BuildSelectClause();
        string orderByClause = orderBy != null ? BuildOrderByClause(orderBy, ascending) : string.Empty;
        var query = string.Format(SelectAllQuery, selectClause, orderByClause);
        return await dbConn.QueryAsync<TEntity>(query);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> wherexpression, Expression<Func<TEntity, object>> orderBy = null, bool ascending = true)
    {
        var where = BuildWhereClause(wherexpression);
        var selectClause = BuildSelectClause();
        string orderByClause = orderBy != null ? BuildOrderByClause(orderBy, ascending) : string.Empty;
        var query = string.Format(SelectByIdQuery, selectClause, where.WhereClause, orderByClause);
        return await dbConn.QueryAsync<TEntity>(query, where.Parameters);
    }

    public async Task<TEntity> GetByIdAsync(Expression<Func<TEntity, bool>> wherexpression, Expression<Func<TEntity, object>> orderBy = null, bool ascending = true)
    {
        var (whereClause, parameters) = BuildWhereClause(wherexpression);
        var selectClause = BuildSelectClause();
        string orderByClause = orderBy != null ? BuildOrderByClause(orderBy, ascending) : string.Empty;
        var query = string.Format(SelectByIdQuery, selectClause, whereClause, orderByClause);
        return await dbConn.QueryFirstOrDefaultAsync<TEntity>(query, parameters);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var updateClause = BuildUpdateClause(entity);
        var whereClause = BuildWhereClauseForKey(entity);
        var query = string.Format(UpdateByIdQuery, updateClause, whereClause);
        await dbConn.ExecuteAsync(query, entity, transaction: EnsureTransaction());
    }

    public async Task UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> wherexpression)
    {
        var updateClause = BuildUpdateClause(entity);
        var where = BuildWhereClause(wherexpression);
        var query = string.Format(UpdateByIdQuery, updateClause, where.WhereClause);
        await dbConn.ExecuteAsync(query, entity, transaction: EnsureTransaction());
    }

    #region Private

    private string BuildSelectClause()
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(decimal) || p.PropertyType == typeof(DateTime) || p.PropertyType.IsEnum);

        return string.Join(", ", properties.Select(p => $"\"{p.Name}\""));
    }

    private (string Columns, string Values) BuildInsertClause(TEntity entity, bool includeKey = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.PropertyType.IsSimpleType())
            .Where(p => p.GetValue(entity) != null && (includeKey || p.GetCustomAttributes(typeof(KeyAttribute), false).Length == 0));

        string columns = string.Join(", ", properties.Select(p => $"\"{p.Name}\""));
        string values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
        return (Columns: columns, Values: values);
    }

    private string BuildUpdateClause(TEntity entity, bool includeKey = false)
    {
        var properties = typeof(TEntity).GetProperties()
            .Where(p => p.PropertyType.IsSimpleType() && (includeKey || !Attribute.IsDefined(p, typeof(KeyAttribute))))
            .Where(p => p.GetValue(entity) != null);

        string setClause = string.Join(", ", properties.Select(p => $"\"{p.Name}\" = @{p.Name}"));
        return setClause;
    }

    private string BuildWhereClauseForKey(TEntity entity)
    {
        var keyProperty = typeof(TEntity).GetProperties()
            .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)));

        if (keyProperty == null)
        {
            throw new InvalidOperationException("No properties with KeyAttribute found on the entity.");
        }

        var keyValue = keyProperty.GetValue(entity);
        if (keyValue == null)
        {
            throw new InvalidOperationException("The key value cannot be null.");
        }

        return $"\"{keyProperty.Name}\" = @{keyProperty.Name}";
    }


    private string BuildOrderByClause<T>(Expression<Func<T, object>> orderByExpression, bool ascending = true)
    {
        if (orderByExpression == null)
        {
            return string.Empty;
        }

        var expressionBody = orderByExpression.Body;

        if (expressionBody is MemberExpression memberExpression)
        {
            return $"ORDER BY \"{memberExpression.Member.Name}\" {(ascending ? "ASC" : "DESC")}";
        }
        else if (expressionBody is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
        {
            return $"ORDER BY \"{((MemberExpression)unaryExpression.Operand).Member.Name}\" {(ascending ? "ASC" : "DESC")}";
        }
        else
        {
            throw new NotSupportedException("The sort expression must contain only property members.");
        }
    }

    private (string WhereClause, DynamicParameters Parameters) BuildWhereClause(Expression<Func<TEntity, bool>> expression)
    {
        var parameters = new DynamicParameters();
        var whereClause = ProcessExpression(expression.Body, parameters);
        return (whereClause, parameters);
    }

    private string ProcessExpression(Expression expression, DynamicParameters parameters)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.AndAlso:
                var andExpr = (BinaryExpression)expression;
                return $"({ProcessExpression(andExpr.Left, parameters)} AND {ProcessExpression(andExpr.Right, parameters)})";

            case ExpressionType.OrElse:
                var orExpr = (BinaryExpression)expression;
                return $"({ProcessExpression(orExpr.Left, parameters)} OR {ProcessExpression(orExpr.Right, parameters)})";

            case ExpressionType.Equal:
                var equalExpr = (BinaryExpression)expression;
                var paramName = GetMemberName(equalExpr.Left);
                var paramValue = GetValue(equalExpr.Right);
                parameters.Add($"@{paramName}", paramValue);
                return $"\"{paramName}\" = @{paramName}";

            case ExpressionType.NotEqual:
                var notEqualExpr = (BinaryExpression)expression;
                paramName = GetMemberName(notEqualExpr.Left);
                paramValue = GetValue(notEqualExpr.Right);
                parameters.Add($"@{paramName}", paramValue);
                return $"\"{paramName}\" != @{paramName}";
            
            case ExpressionType.Call:
                var methodCallExpr = (MethodCallExpression)expression;
                return ProcessMethodCall(methodCallExpr, parameters);

            default:
                throw new NotSupportedException($"Operation '{expression.NodeType}' not supported.");
        }
    }

    private string ProcessMethodCall(MethodCallExpression expression, DynamicParameters parameters)
    {
        var methodName = expression.Method.Name;
        var propertyName = ((MemberExpression)expression.Object).Member.Name;

        var argument = expression.Arguments[0];
        var value = GetValue(argument);

        var paramName = $"@{propertyName}_{parameters.ParameterNames.Count()}";

        switch (methodName)
        {
            case "Contains":
                parameters.Add(paramName, $"%{value}%");
                return $"\"{propertyName}\" LIKE {paramName}";

            case "StartsWith":
                parameters.Add(paramName, $"{value}%");
                return $"\"{propertyName}\" LIKE {paramName}";

            case "EndsWith":
                parameters.Add(paramName, $"%{value}");
                return $"\"{propertyName}\" LIKE {paramName}";

            default:
                throw new NotSupportedException($"Method '{methodName}' is not supported.");
        }
    }


    private string GetMemberName(Expression expression)
    {
        if (expression is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new InvalidOperationException("Expression must be of type MemberExpression.");
    }

    private object GetValue(Expression expression)
    {
        switch (expression)
        {
            case ConstantExpression constant:
                return constant.Value;

            case MemberExpression memberExpression:
                var objectMember = Expression.Convert(memberExpression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return getter();

            case UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert:
                return GetValue(unaryExpression.Operand);

            default:
                throw new InvalidOperationException("Unsupported expression type: " + expression.GetType().FullName);
        }
    }

    #endregion
}
