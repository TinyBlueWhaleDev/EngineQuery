using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TinyBlueWhale.EngineQuery.Abstractions.Enums;
using TinyBlueWhale.EngineQuery.Tests.Models;

namespace TinyBlueWhale.EngineQuery.Tests.Infrastructure
{
    /// <summary>
    /// Provides provider-shared snapshot tests for successful SQL generation features.
    /// </summary>
    public abstract class QueryCompilerFeatureSnapshotTests : QueryCompilerProviderTestBase
    {
        /// <summary>
        /// Validates INSERT SELECT target column inference from the SELECT projection.
        /// </summary>
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Insert_Select_With_Inferred_Columns()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();

            AssertSnapshot(
                "insert_select_inferred_columns",
                sql);
        }

        /// <summary>
        /// Validates INSERT SELECT target column inference from projection property names when aliases are not configured.
        /// </summary>
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Insert_Select_With_Inferred_Property_Names()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build();

            AssertSnapshot(
                "insert_select_inferred_property_names",
                sql);
        }

        /// <summary>
        /// Validates that INSERT SELECT column inference requires at least one SELECT projection.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Inferred_Columns_Have_No_Select_Projection()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: "u")
                .Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one SELECT projection must be configured when INSERT target columns are not explicitly configured."));
        }

        /// <summary>
        /// Validates that INSERT SELECT inferred target columns cannot resolve the same projection name more than once.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Inferred_Target_Column_Is_Duplicated()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    UserId = order.UserId
                })
                .Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("Target INSERT column 'UserId' was resolved more than once from the SELECT projection."));
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Insert_Select_With_Join()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();

            AssertSnapshot(
                "insert_select_join",
                sql);
        }

        /// <summary>
        /// Validates that INSERT SELECT preserves the INSERT target while using the configured source as the query root.
        /// </summary>
        [Test]
        public void ToSql_Should_Preserve_Insert_Target_When_Select_Source_Uses_A_Different_Entity()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Build();

            Assert.That(sql.CommandText, Does.Contain("orders"));
            Assert.That(sql.CommandText, Does.Contain("users"));
        }

        /// <summary>
        /// Validates that INSERT value assignments cannot be combined with an INSERT SELECT source.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Value_Assignments_Were_Configured_First()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com")
                .From<JoinUser>(alias: "u"));

            Assert.That(exception!.Message, Is.EqualTo("An INSERT SELECT source cannot be combined with INSERT value assignments."));
        }

        /// <summary>
        /// Validates that INSERT value assignments cannot be added after configuring an INSERT SELECT source.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Value_Assignment_Is_Added_After_Source()
        {
            var queryBuilder = CreateQueryBuilder();

            var commandBuilder = queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: "u");

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder
                .Set(user => user.Email, "admin@test.com"));

            Assert.That(exception!.Message, Is.EqualTo("INSERT value assignments cannot be combined with an INSERT SELECT source."));
        }

        /// <summary>
        /// Validates that an INSERT SELECT command cannot configure more than one root source.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_A_Second_Source_Is_Configured()
        {
            var queryBuilder = CreateQueryBuilder();

            var commandBuilder = queryBuilder
                .InsertInto<JoinOrder>()
                .From<JoinUser>(alias: "u");

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder
                .From<JoinOrder>(alias: "o"));

            Assert.That(exception!.Message, Is.EqualTo("The INSERT SELECT source is already configured."));
        }

        /// <summary>
        /// Validates that an INSERT command requires either value assignments or a SELECT source before compilation.
        /// </summary>
        [Test]
        public void Insert_Should_Throw_When_No_Values_Or_Select_Source_Are_Configured()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one value or SELECT source must be configured before building an INSERT command."));
        }

        /// <summary>
        /// Validates that explicit INSERT SELECT target columns cannot be combined with INSERT value assignments.
        /// </summary>
        [Test]
        public void Insert_Should_Throw_When_Columns_Are_Added_After_Value_Assignments()
        {
            var queryBuilder = CreateQueryBuilder();

            var commandBuilder = queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "admin@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                }));

            Assert.That(exception!.Message, Is.EqualTo("INSERT SELECT columns cannot be combined with INSERT value assignments."));
        }

        /// <summary>
        /// Validates that INSERT value assignments cannot be combined with explicitly configured INSERT SELECT target columns.
        /// </summary>
        [Test]
        public void Insert_Should_Throw_When_Value_Assignment_Is_Added_After_Columns()
        {
            var queryBuilder = CreateQueryBuilder();

            var commandBuilder = queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                });

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder
                .Set(user => user.Email, "admin@test.com"));

            Assert.That(exception!.Message, Is.EqualTo("INSERT value assignments cannot be combined with explicitly configured INSERT SELECT columns."));
        }

        /// <summary>
        /// Validates that the same target INSERT column cannot be configured more than once.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Target_Column_Is_Configured_More_Than_Once()
        {
            var queryBuilder = CreateQueryBuilder();

            var commandBuilder = queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                });

            var exception = Assert.Throws<InvalidOperationException>(() => commandBuilder
                .Columns(user => user.Email));

            Assert.That(exception!.Message, Is.EqualTo("Property 'Email' is already configured as an INSERT target column."));
        }

        /// <summary>
        /// Validates that INSERT SELECT target column expressions cannot be null.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Columns_Selector_Is_Null()
        {
            var queryBuilder = CreateQueryBuilder();

            Expression<Func<JoinUser, object>> selector = null!;

            Assert.Throws<ArgumentNullException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Columns(selector));
        }

        /// <summary>
        /// Validates that INSERT SELECT target columns must reference direct entity properties.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Columns_Selector_Does_Not_Reference_Direct_Properties()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<ArgumentException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    Value = user.Email!.Length
                }));

            Assert.That(exception!.Message, Does.Contain("The INSERT columns selector must reference direct entity properties."));
        }

        /// <summary>
        /// Validates that INSERT SELECT source aliases cannot contain whitespace-only values.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Source_Alias_Is_Whitespace()
        {
            var queryBuilder = CreateQueryBuilder();

            Assert.Throws<ArgumentException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>(alias: " "));
        }

        /// <summary>
        /// Validates that INSERT SELECT explicit source table names cannot be empty.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Throw_When_Source_Table_Name_Is_Empty()
        {
            var queryBuilder = CreateQueryBuilder();

            Assert.Throws<ArgumentException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .From<JoinUser>("", alias: "u"));
        }

        /// <summary>
        /// Validates that adding joined sources does not replace the configured INSERT SELECT root source.
        /// </summary>
        [Test]
        public void Insert_Select_Should_Preserve_Root_Source_After_Join()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<JoinOrder>()
                .Columns(order => new
                {
                    order.UserId,
                    order.Total
                })
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(alias: "o", on: (user, order) => user.Id == order.UserId)
                .Select<JoinUser>(user => new
                {
                    UserId = user.Id
                })
                .Select<JoinOrder>(order => new
                {
                    order.Total
                })
                .Build();

            AssertSnapshot(
                "insert_select_join_root_source",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Insert_Select()
        {
            var query = CreateQueryBuilder()
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(user => new
                {
                    user.Id,
                    user.Email
                })
                .Where<JoinUser>(user => user.IsActive)
                .Build();

            AssertSnapshot(
                "insert_select",
                query);
        }

        [Test]
        public void Build_Should_Throw_When_Insert_Values_And_Select_Source_Are_Combined()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Set(user => user.Email, "user@test.com")
                .From<JoinUser>(alias: "u")
                .Build());

            Assert.That(exception!.Message, Is.EqualTo("An INSERT SELECT source cannot be combined with INSERT value assignments."));
        }

        [Test]
        public void Build_Should_Throw_When_Insert_Select_Source_Is_Not_Configured()
        {
            var queryBuilder = CreateQueryBuilder();

            var exception = Assert.Throws<InvalidOperationException>(() => queryBuilder
                .InsertInto<JoinUser>()
                .Columns(user => new
                {
                    user.Id,
                    user.Email
                })
                .Build());

            Assert.That(exception!.Message, Is.EqualTo("At least one value or SELECT source must be configured before building an INSERT command."));
        }

        /// <summary>
        /// Validates SQL generation for strongly typed DELETE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Delete()
        {
            var sql = CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Build();

            AssertSnapshot(
                "delete",
                sql);
        }

        /// <summary>
        /// Validates parameter generation for strongly typed DELETE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Delete_Parameters_In_Clause_Order()
        {
            var sql = CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Id == 10)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
                Assert.That(sql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo(10));
                Assert.That(sql.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo(true));
            });
        }

        /// <summary>
        /// Validates conditional WHERE generation for strongly typed DELETE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Delete_With_WhereIf()
        {
            var sql = CreateQueryBuilder()
                .DeleteFrom<User>()
                .WhereIf(true, user => user.Age >= 18)
                .WhereIf(false, user => user.IsDeleted)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Does.Contain("Age"));
                Assert.That(sql.CommandText, Does.Not.Contain("IsDeleted"));
                Assert.That(sql.Parameters, Has.Count.EqualTo(1));
            });
        }

        /// <summary>
        /// Validates logical operator composition for DELETE WHERE predicates.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Delete_With_Logical_Operators()
        {
            var sql = CreateQueryBuilder()
                .DeleteFrom<User>()
                .Where(user => user.Age < 18)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.That(sql.CommandText, Does.Contain(" OR "));
        }

        /// <summary>
        /// Validates DELETE command generation using an explicit table name.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Delete_Using_Explicit_Table_Name()
        {
            var sql = CreateQueryBuilder()
                .DeleteFrom<User>("CustomUsers")
                .Where(user => user.Id == 10)
                .Build();

            Assert.That(sql.CommandText, Does.Contain("CustomUsers"));
        }

        /// <summary>
        /// Validates that DELETE commands require at least one WHERE predicate.
        /// </summary>
        [Test]
        public void Delete_Should_Reject_Command_Without_Where_Predicate()
        {
            var command = CreateQueryBuilder()
                .DeleteFrom<User>();

            var exception = Assert.Throws<InvalidOperationException>(() => command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one WHERE predicate must be configured before building a DELETE command."));
        }

        /// <summary>
        /// Validates nullable UPDATE value generation.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Update_With_Null_Value()
        {
            var sql = CreateQueryBuilder()
                .Update<Category>()
                .Set(category => category.ParentId, null)
                .Where(category => category.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
                Assert.That(sql.Parameters[0].Value, Is.Null);
                Assert.That(sql.Parameters[1].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates that UPDATE assignments only accept direct entity properties.
        /// </summary>
        [Test]
        public void Update_Should_Reject_Non_Property_Selector()
        {
            var command = CreateQueryBuilder()
                .Update<User>();

            var exception = Assert.Throws<ArgumentException>(() =>
                command.Set(user => user.Email.ToLower(), "updated@test.com"));

            Assert.That(
                exception!.Message,
                Does.StartWith("The UPDATE selector must reference a direct entity property."));
        }

        /// <summary>
        /// Validates that an UPDATE column cannot receive multiple value assignments.
        /// </summary>
        [Test]
        public void Update_Should_Reject_Duplicate_Value_Assignments()
        {
            var command = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "first@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                command.Set(user => user.Email, "second@test.com"));

            Assert.That(
                exception!.Message,
                Is.EqualTo("Column 'Email' already has an UPDATE value assignment."));
        }

        /// <summary>
        /// Validates that UPDATE commands require at least one WHERE predicate.
        /// </summary>
        [Test]
        public void Update_Should_Reject_Command_Without_Where_Predicate()
        {
            var command = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one WHERE predicate must be configured before building an UPDATE command."));
        }

        /// <summary>
        /// Validates that UPDATE commands require at least one value assignment.
        /// </summary>
        [Test]
        public void Update_Should_Reject_Command_Without_Value_Assignments()
        {
            var command = CreateQueryBuilder()
                .Update<User>()
                .Where(user => user.Id == 10);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                command.Build());

            Assert.That(
                exception!.Message,
                Is.EqualTo("At least one value must be configured before building an UPDATE command."));
        }

        /// <summary>
        /// Validates UPDATE command generation using an explicit table name.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Update_Using_Explicit_Table_Name()
        {
            var sql = CreateQueryBuilder()
                .Update<User>("CustomUsers")
                .Set(user => user.Email, "updated@test.com")
                .Where(user => user.Id == 10)
                .Build();

            Assert.That(
                sql.CommandText,
                Does.Contain("CustomUsers"));
        }

        /// <summary>
        /// Validates logical operator composition for UPDATE WHERE predicates.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Update_With_Logical_Operators()
        {
            var sql = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.IsActive, false)
                .Where(user => user.Age < 18)
                .Where(user => user.IsDeleted, QueryLogicalOperator.Or)
                .Build();

            Assert.That(
                sql.CommandText,
                Does.Contain(" OR "));
        }

        /// <summary>
        /// Validates conditional WHERE generation for strongly typed UPDATE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Update_With_WhereIf()
        {
            var sql = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.IsActive, false)
                .WhereIf(true, user => user.Age >= 18)
                .WhereIf(false, user => user.IsDeleted)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.CommandText, Does.Contain("Age"));
                Assert.That(sql.CommandText, Does.Not.Contain("IsDeleted"));
                Assert.That(sql.Parameters, Has.Count.EqualTo(2));
            });
        }

        /// <summary>
        /// Validates SQL generation for strongly typed UPDATE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Update()
        {
            var sql = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            AssertSnapshot(
                "update",
                sql);
        }

        /// <summary>
        /// Validates parameter generation for strongly typed UPDATE commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Update_Parameters_In_Clause_Order()
        {
            var sql = CreateQueryBuilder()
                .Update<User>()
                .Set(user => user.Email, "updated@test.com")
                .Set(user => user.IsActive, false)
                .Where(user => user.Id == 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(3));
                Assert.That(sql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("updated@test.com"));
                Assert.That(sql.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo(false));
                Assert.That(sql.Parameters[2].Name, Is.EqualTo("@p2"));
                Assert.That(sql.Parameters[2].Value, Is.EqualTo(10));
            });
        }

        /// <summary>
        /// Validates SQL generation for strongly typed INSERT commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Match_Snapshot_For_Insert()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.Age, 35)
                .Set(user => user.IsActive, true)
                .Build();

            AssertSnapshot(
                "insert",
                sql);
        }

        /// <summary>
        /// Validates parameter generation for strongly typed INSERT commands.
        /// </summary>
        [Test]
        public void ToSql_Should_Generate_Insert_Parameters_In_Assignment_Order()
        {
            var sql = CreateQueryBuilder()
                .InsertInto<User>()
                .Set(user => user.Email, "admin@test.com")
                .Set(user => user.Age, 35)
                .Set(user => user.IsActive, true)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql.Parameters, Has.Count.EqualTo(3));
                Assert.That(sql.Parameters[0].Name, Is.EqualTo("@p0"));
                Assert.That(sql.Parameters[0].Value, Is.EqualTo("admin@test.com"));
                Assert.That(sql.Parameters[1].Name, Is.EqualTo("@p1"));
                Assert.That(sql.Parameters[1].Value, Is.EqualTo(35));
                Assert.That(sql.Parameters[2].Name, Is.EqualTo("@p2"));
                Assert.That(sql.Parameters[2].Value, Is.EqualTo(true));
            });
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Select_All()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Build();

            AssertSnapshot(
                "select_all",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Select_Projection()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Select<User>(x => new
                {
                    x.Id,
                    x.Email
                })
                .Build();

            AssertSnapshot(
                "select_projection",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Where_Boolean_And_String_Methods()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x =>
                    x.IsActive &&
                    x.Email.Contains("@gmail.com") &&
                    x.Age >= 18)
                .Build();

            AssertSnapshot(
                "where_boolean_string_methods",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Where_Or()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x =>
                    x.Email.Contains("@gmail.com") ||
                    x.Email.Contains("@company.com"))
                .Build();

            AssertSnapshot(
                "where_or",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_WhereIf()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .WhereIf<User>(true, x => x.IsActive)
                .WhereIf<User>(false, x => x.IsDeleted)
                .Build();

            AssertSnapshot(
                "where_if",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Order_And_Pagination()
        {
            var sql = CreateQueryBuilder()
                .From<User>("Users")
                .OrderBy<User>(x => x.Email)
                .ThenByDescending<User>(x => x.CreatedAt)
                .Skip(20)
                .Take(10)
                .Build();

            AssertSnapshot(
                "order_pagination",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Distinct()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Distinct()
                .Select<JoinUser>(u => new
                {
                    u.Email
                })
                .Build();

            AssertSnapshot(
                "distinct",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Joins()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .LeftJoin<JoinOrder, JoinOrderItem>(
                    alias: "oi",
                    on: (o, oi) => o.Id == oi.OrderId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .Build();

            AssertSnapshot(
                "joins",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_GroupBy_Aggregates_And_Having()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .InnerJoin<JoinUser, JoinOrder>(
                    alias: "o",
                    on: (u, o) => u.Id == o.UserId)
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    "TotalAmount")
                .SelectAggregate<JoinOrder>(
                    QueryAggregateFunction.Count,
                    o => o.Id,
                    "OrderCount")
                .GroupBy<JoinUser>(u => new
                {
                    u.Id,
                    u.Email
                })
                .HavingAggregate<JoinOrder>(
                    QueryAggregateFunction.Sum,
                    o => o.Total,
                    QueryComparisonOperator.GreaterThan,
                    1000)
                .Build();

            AssertSnapshot(
                "groupby_aggregate_having",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Scalar_Functions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id
                })
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Upper,
                    u => u.Email,
                    "NormalizedEmail")
                .SelectScalarFunction<JoinUser>(
                    QueryScalarFunction.Length,
                    u => u.Email,
                    "EmailLength")
                .Build();

            AssertSnapshot(
                "scalar_functions",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Computed_Expressions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectComputed<JoinOrder>(
                    o => o.Total * 1.16m,
                    "TotalWithTax")
                .WhereComputed<JoinOrder>(
                    o => o.Total * 1.16m > 1000)
                .Build();

            AssertSnapshot(
                "computed_expressions",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Case_When()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.Total
                })
                .SelectCaseWhen<JoinOrder>(
                    condition: o => o.Total > 1000,
                    whenTrue: "VIP",
                    whenFalse: "STANDARD",
                    alias: "CustomerType")
                .Build();

            AssertSnapshot(
                "case_when",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Exists_NotExists_And_In()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .WhereExists<JoinUser, JoinOrder>(
                    alias: "o",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id &&
                            o.Total > 100))
                .WhereNotExists<JoinUser, JoinOrder>(
                    alias: "o2",
                    subquery => subquery
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id &&
                            o.Total <= 0))
                .WhereIn<JoinUser, JoinOrder>(
                    u => u.Id,
                    alias: "oi",
                    subquery => subquery
                        .Select<JoinOrder>(o => new
                        {
                            o.UserId
                        })
                        .Where<JoinOrder>(o => o.Total > 500))
                .Build();

            AssertSnapshot(
                "exists_notexists_in",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Derived_Table()
        {
            var sql = CreateQueryBuilder()
                .FromSubquery<OrderSummary, JoinOrder>(
                    alias: "summary",
                    subquery => subquery
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();

            AssertSnapshot(
                "derived_table",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Cte()
        {
            var sql = CreateQueryBuilder()
                .With<OrderSummary, JoinOrder>(
                    "order_summary",
                    cte => cte
                        .From<JoinOrder>(alias: "o")
                        .Select<JoinOrder>(o => new
                        {
                            UserId = o.UserId
                        })
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Sum,
                            o => o.Total,
                            "TotalAmount")
                        .SelectAggregate<JoinOrder>(
                            QueryAggregateFunction.Count,
                            o => o.Id,
                            "OrderCount")
                        .GroupBy<JoinOrder>(o => o.UserId))
                .FromCte<OrderSummary>("order_summary")
                .Select<OrderSummary>(summary => new
                {
                    summary.UserId,
                    summary.TotalAmount,
                    summary.OrderCount
                })
                .Build();

            AssertSnapshot(
                "cte",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Recursive_Cte()
        {
            var sql = CreateQueryBuilder()
                .WithRecursive<CategoryTree, Category, Category>(
                    name: "category_tree",
                    baseQueryBuilder: baseQuery => baseQuery
                        .From<Category>(alias: "c")
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        })
                        .Where<Category>(c => c.ParentId == null),
                    recursiveQueryBuilder: recursiveQuery => recursiveQuery
                        .From<Category>(alias: "c")
                        .InnerJoin<Category, CategoryTree>(
                            alias: "ct",
                            on: (c, ct) => c.ParentId == ct.Id)
                        .Select<Category>(c => new
                        {
                            c.Id,
                            c.ParentId,
                            c.Name
                        }))
                .FromCte<CategoryTree>("category_tree")
                .Select<CategoryTree>(tree => new
                {
                    tree.Id,
                    tree.ParentId,
                    tree.Name
                })
                .Build();

            AssertSnapshot(
                "recursive_cte",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Set_Operations()
        {
            var sql = CreateQueryBuilder()
                .From<ActiveUser>(alias: "u")
                .Select<ActiveUser>(u => new
                {
                    u.Email
                })
                .UnionAll<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Intersect<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a2")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Except<ArchivedUser>(set => set
                    .From<ArchivedUser>(alias: "a3")
                    .Select<ArchivedUser>(a => new
                    {
                        a.Email
                    }))
                .Build();

            AssertSnapshot(
                "set_operations",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Apply_Lateral()
        {
            var sql = CreateQueryBuilder()
                .From<JoinUser>(alias: "u")
                .Select<JoinUser>(u => new
                {
                    UserId = u.Id,
                    u.Email
                })
                .CrossApply<JoinUser, JoinOrder>(
                    alias: "latest_order",
                    apply => apply
                        .Select<JoinOrder>(o => new
                        {
                            OrderId = o.Id,
                            o.UserId,
                            o.Total
                        })
                        .WhereComputed<JoinOrder, JoinUser>((o, u) =>
                            o.UserId == u.Id)
                        .OrderByDescending<JoinOrder>(o => o.Total)
                        .Take(1))
                .Build();

            AssertSnapshot(
                "apply_lateral",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Window_Functions()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .Select<JoinOrder>(o => new
                {
                    OrderId = o.Id,
                    o.UserId,
                    o.Total
                })
                .SelectRowNumber(
                    "RowNumber",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectRank(
                    "OrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectDenseRank(
                    "DenseOrderRank",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .SelectLag<JoinOrder>(
                    o => o.Total,
                    "PreviousOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLead<JoinOrder>(
                    o => o.Total,
                    "NextOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectFirstValue<JoinOrder>(
                    o => o.Total,
                    "FirstOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectLastValue<JoinOrder>(
                    o => o.Total,
                    "LastOrderTotal",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderBy<JoinOrder>(o => o.Id))
                .SelectNtile(
                    4,
                    "OrderQuartile",
                    window => window
                        .PartitionBy<JoinOrder>(o => o.UserId)
                        .OrderByDescending<JoinOrder>(o => o.Total))
                .Build();

            AssertSnapshot(
                "window_functions",
                sql);
        }

        [Test]
        public void ToSql_Should_Be_Deterministic()
        {
            var query = CreateQueryBuilder()
                .From<User>("Users")
                .Where<User>(x => x.IsActive);

            var sql1 = query.Build();
            var sql2 = query.Build();

            Assert.Multiple(() =>
            {
                Assert.That(sql1.CommandText, Is.EqualTo(sql2.CommandText));
                Assert.That(sql1.Parameters.Count, Is.EqualTo(sql2.Parameters.Count));
                Assert.That(sql1.Parameters[0].Name, Is.EqualTo(sql2.Parameters[0].Name));
                Assert.That(sql1.Parameters[0].Value, Is.EqualTo(sql2.Parameters[0].Value));
            });
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Coalesce_Computed_Expression()
        {
            var sql = CreateQueryBuilder()
                .From<User>(alias: "u")
                .SelectComputed<User>(
                    u => u.Email ?? string.Empty,
                    "Nombre")
                .Build();

            AssertSnapshot(
                "coalesce_computed_expression",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Compound_Join_AndAlso()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .LeftJoin<JoinOrder, JoinUser>(
                    alias: "u",
                    on: (order, user) =>
                        order.UserId == user.Id &&
                        order.TenantId == user.TenantId)
                .Select<JoinOrder>(order => new
                {
                    order.Id,
                    order.UserId
                })
                .Select<JoinUser>(user => new
                {
                    UserEmail = user.Email
                })
                .Build();

            AssertSnapshot(
                "compound_join_andalso",
                sql);
        }

        [Test]
        public void ToSql_Should_Match_Snapshot_For_Compound_Join_OrElse()
        {
            var sql = CreateQueryBuilder()
                .From<JoinOrder>(alias: "o")
                .LeftJoin<JoinOrder, JoinUser>(
                    alias: "u",
                    on: (order, user) =>
                        order.UserId == user.Id ||
                        order.ApproverUserId == user.Id)
                .Select<JoinOrder>(order => new
                {
                    order.Id,
                    order.UserId
                })
                .Select<JoinUser>(user => new
                {
                    UserEmail = user.Email
                })
                .Build();

            AssertSnapshot(
                "compound_join_orelse",
                sql);
        }

    }
}
