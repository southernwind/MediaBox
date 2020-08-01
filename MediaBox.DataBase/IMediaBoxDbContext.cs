using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public interface IMediaBoxDbContext {
		/// <summary>
		/// アルバムテーブル
		/// </summary>
		public DbSet<Album> Albums {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックステーブル
		/// </summary>
		public DbSet<AlbumBox> AlbumBoxes {
			get;
			set;
		}

		/// <summary>
		/// アルバムディレクトリ
		/// </summary>
		public DbSet<AlbumScanDirectory> AlbumScanDirectories {
			get;
			set;
		}

		/// <summary>
		/// アルバム・メディアファイル中間テーブル
		/// </summary>
		public DbSet<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

		//
		// 概要:
		//     Provides access to information and operations for entity instances this context
		//     is tracking.
		public ChangeTracker ChangeTracker {
			get;
		}
		//
		// 概要:
		//     Provides access to database related information and operations for this context.
		public DatabaseFacade Database {
			get;
		}
		//
		// 概要:
		//     The metadata about the shape of entities, the relationships between them, and
		//     how they map to the database.
		public IModel Model {
			get;
		}
		//
		// 概要:
		//     A unique identifier for the context instance and pool lease, if any.
		//     This identifier is primarily intended as a correlation ID for logging and debugging
		//     such that it is easy to identify that multiple events are using the same or different
		//     context instances.
		public DbContextId ContextId {
			get;
		}

		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to add.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public EntityEntry Add([NotNull] object entity);
		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to add.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of the entity.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1 for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public EntityEntry<TEntity> Add<TEntity>([NotNull] TEntity entity) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//     This method is async only to allow special value generators, such as the one
		//     used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
		//     to access the database asynchronously. For all other cases the non async method
		//     should be used.
		//
		// パラメーター:
		//   entity:
		//     The entity to add.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 戻り値:
		//     A task that represents the asynchronous Add operation. The task result contains
		//     the Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public ValueTask<EntityEntry> AddAsync([NotNull] object entity, CancellationToken cancellationToken = default);
		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     This method is async only to allow special value generators, such as the one
		//     used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
		//     to access the database asynchronously. For all other cases the non async method
		//     should be used.
		//
		// パラメーター:
		//   entity:
		//     The entity to add.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of the entity.
		//
		// 戻り値:
		//     A task that represents the asynchronous Add operation. The task result contains
		//     the Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1 for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>([NotNull] TEntity entity, CancellationToken cancellationToken = default) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entities, and any other reachable entities that are
		//     not already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entities:
		//     The entities to add.
		public void AddRange([NotNull] IEnumerable<object> entities);
		//
		// 概要:
		//     Begins tracking the given entities, and any other reachable entities that are
		//     not already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entities:
		//     The entities to add.
		public void AddRange([NotNull] params object[] entities);
		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     This method is async only to allow special value generators, such as the one
		//     used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
		//     to access the database asynchronously. For all other cases the non async method
		//     should be used.
		//
		// パラメーター:
		//   entities:
		//     The entities to add.
		//
		// 戻り値:
		//     A task that represents the asynchronous operation.
		public Task AddRangeAsync([NotNull] params object[] entities);
		//
		// 概要:
		//     Begins tracking the given entity, and any other reachable entities that are not
		//     already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     This method is async only to allow special value generators, such as the one
		//     used by 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
		//     to access the database asynchronously. For all other cases the non async method
		//     should be used.
		//
		// パラメーター:
		//   entities:
		//     The entities to add.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 戻り値:
		//     A task that represents the asynchronous operation.
		public Task AddRangeAsync([NotNull] IEnumerable<object> entities, CancellationToken cancellationToken = default);
		//
		// 概要:
		//     Begins tracking the given entity and entries reachable from the given entity
		//     using the Microsoft.EntityFrameworkCore.EntityState.Unchanged state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure only new entities will be inserted. An entity is considered
		//     to have its primary key value set if the primary key property is set to anything
		//     other than the CLR default for the property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Unchanged.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to attach.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public EntityEntry Attach([NotNull] object entity);
		//
		// 概要:
		//     Begins tracking the given entity and entries reachable from the given entity
		//     using the Microsoft.EntityFrameworkCore.EntityState.Unchanged state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure only new entities will be inserted. An entity is considered
		//     to have its primary key value set if the primary key property is set to anything
		//     other than the CLR default for the property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Unchanged.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to attach.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of the entity.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1 for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public EntityEntry<TEntity> Attach<TEntity>([NotNull] TEntity entity) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entities and entries reachable from the given entities
		//     using the Microsoft.EntityFrameworkCore.EntityState.Unchanged state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure only new entities will be inserted. An entity is considered
		//     to have its primary key value set if the primary key property is set to anything
		//     other than the CLR default for the property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Unchanged.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entities:
		//     The entities to attach.
		public void AttachRange([NotNull] IEnumerable<object> entities);
		//
		// 概要:
		//     Begins tracking the given entities and entries reachable from the given entities
		//     using the Microsoft.EntityFrameworkCore.EntityState.Unchanged state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure only new entities will be inserted. An entity is considered
		//     to have its primary key value set if the primary key property is set to anything
		//     other than the CLR default for the property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Unchanged.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entities:
		//     The entities to attach.
		public void AttachRange([NotNull] params object[] entities);
		//
		// 概要:
		//     Releases the allocated resources for this context.
		public void Dispose();
		//
		// 概要:
		//     Releases the allocated resources for this context.
		public ValueTask DisposeAsync();
		//
		// 概要:
		//     Gets an Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1 for the given
		//     entity. The entry provides access to change tracking information and operations
		//     for the entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to get the entry for.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of the entity.
		//
		// 戻り値:
		//     The entry for the given entity.
		public EntityEntry<TEntity> Entry<TEntity>([NotNull] TEntity entity) where TEntity : class;
		//
		// 概要:
		//     Gets an Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the given
		//     entity. The entry provides access to change tracking information and operations
		//     for the entity.
		//     This method may be called on an entity that is not tracked. You can then set
		//     the Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State property on
		//     the returned entry to have the context begin tracking the entity in the specified
		//     state.
		//
		// パラメーター:
		//   entity:
		//     The entity to get the entry for.
		//
		// 戻り値:
		//     The entry for the given entity.
		public EntityEntry Entry([NotNull] object entity);
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of entity to find.
		//
		// 戻り値:
		//     The entity found, or null.
		public TEntity Find<TEntity>([MaybeNull] params object[] keyValues) where TEntity : class;
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   entityType:
		//     The type of entity to find.
		//
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		// 戻り値:
		//     The entity found, or null.
		public object Find([NotNull] Type entityType, [MaybeNull] params object[] keyValues);
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   entityType:
		//     The type of entity to find.
		//
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 戻り値:
		//     The entity found, or null.
		public ValueTask<object> FindAsync([NotNull] Type entityType, [MaybeNull] object[] keyValues, CancellationToken cancellationToken);
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   entityType:
		//     The type of entity to find.
		//
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		// 戻り値:
		//     The entity found, or null.
		public ValueTask<object> FindAsync([NotNull] Type entityType, [MaybeNull] params object[] keyValues);
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of entity to find.
		//
		// 戻り値:
		//     The entity found, or null.
		public ValueTask<TEntity> FindAsync<TEntity>([MaybeNull] object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
		//
		// 概要:
		//     Finds an entity with the given primary key values. If an entity with the given
		//     primary key values is being tracked by the context, then it is returned immediately
		//     without making a request to the database. Otherwise, a query is made to the database
		//     for an entity with the given primary key values and this entity, if found, is
		//     attached to the context and returned. If no entity is found, then null is returned.
		//
		// パラメーター:
		//   keyValues:
		//     The values of the primary key for the entity to be found.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of entity to find.
		//
		// 戻り値:
		//     The entity found, or null.
		public ValueTask<TEntity> FindAsync<TEntity>([MaybeNull] params object[] keyValues) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		//     state such that it will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entity:
		//     The entity to remove.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		//
		// 注釈:
		//     If the entity is already tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state then the context will stop tracking the entity (rather than marking it
		//     as Microsoft.EntityFrameworkCore.EntityState.Deleted) since the entity was previously
		//     added to the context and does not exist in the database.
		//     Any other reachable entities that are not already being tracked will be tracked
		//     in the same way that they would be if Microsoft.EntityFrameworkCore.DbContext.Attach(System.Object)
		//     was called before calling this method. This allows any cascading actions to be
		//     applied when Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		public EntityEntry Remove([NotNull] object entity);
		//
		// 概要:
		//     Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		//     state such that it will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entity:
		//     The entity to remove.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of the entity.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1 for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		//
		// 注釈:
		//     If the entity is already tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state then the context will stop tracking the entity (rather than marking it
		//     as Microsoft.EntityFrameworkCore.EntityState.Deleted) since the entity was previously
		//     added to the context and does not exist in the database.
		//     Any other reachable entities that are not already being tracked will be tracked
		//     in the same way that they would be if Microsoft.EntityFrameworkCore.DbContext.Attach``1(``0)
		//     was called before calling this method. This allows any cascading actions to be
		//     applied when Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		public EntityEntry<TEntity> Remove<TEntity>([NotNull] TEntity entity) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		//     state such that it will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entities:
		//     The entities to remove.
		//
		// 注釈:
		//     If any of the entities are already tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state then the context will stop tracking those entities (rather than marking
		//     them as Microsoft.EntityFrameworkCore.EntityState.Deleted) since those entities
		//     were previously added to the context and do not exist in the database.
		//     Any other reachable entities that are not already being tracked will be tracked
		//     in the same way that they would be if Microsoft.EntityFrameworkCore.DbContext.AttachRange(System.Collections.Generic.IEnumerable{System.Object})
		//     was called before calling this method. This allows any cascading actions to be
		//     applied when Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called.
		public void RemoveRange([NotNull] IEnumerable<object> entities);
		//
		// 概要:
		//     Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		//     state such that it will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//
		// パラメーター:
		//   entities:
		//     The entities to remove.
		//
		// 注釈:
		//     If any of the entities are already tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state then the context will stop tracking those entities (rather than marking
		//     them as Microsoft.EntityFrameworkCore.EntityState.Deleted) since those entities
		//     were previously added to the context and do not exist in the database.
		//     Any other reachable entities that are not already being tracked will be tracked
		//     in the same way that they would be if Microsoft.EntityFrameworkCore.DbContext.AttachRange(System.Object[])
		//     was called before calling this method. This allows any cascading actions to be
		//     applied when Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called.
		public void RemoveRange([NotNull] params object[] entities);
		//
		// 概要:
		//     Saves all changes made in this context to the database.
		//     This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
		//     to discover any changes to entity instances before saving to the underlying database.
		//     This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
		//
		// 戻り値:
		//     The number of state entries written to the database.
		//
		// 例外:
		//   T:Microsoft.EntityFrameworkCore.DbUpdateException:
		//     An error is encountered while saving to the database.
		//
		//   T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
		//     A concurrency violation is encountered while saving to the database. A concurrency
		//     violation occurs when an unexpected number of rows are affected during save.
		//     This is usually because the data in the database has been modified since it was
		//     loaded into memory.
		public int SaveChanges();
		//
		// 概要:
		//     Saves all changes made in this context to the database.
		//     This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
		//     to discover any changes to entity instances before saving to the underlying database.
		//     This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
		//
		// パラメーター:
		//   acceptAllChangesOnSuccess:
		//     Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
		//     is called after the changes have been sent successfully to the database.
		//
		// 戻り値:
		//     The number of state entries written to the database.
		//
		// 例外:
		//   T:Microsoft.EntityFrameworkCore.DbUpdateException:
		//     An error is encountered while saving to the database.
		//
		//   T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
		//     A concurrency violation is encountered while saving to the database. A concurrency
		//     violation occurs when an unexpected number of rows are affected during save.
		//     This is usually because the data in the database has been modified since it was
		//     loaded into memory.
		public int SaveChanges(bool acceptAllChangesOnSuccess);
		//
		// 概要:
		//     Saves all changes made in this context to the database.
		//     This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
		//     to discover any changes to entity instances before saving to the underlying database.
		//     This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
		//     Multiple active operations on the same context instance are not supported. Use
		//     'await' to ensure that any asynchronous operations have completed before calling
		//     another method on this context.
		//
		// パラメーター:
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 戻り値:
		//     A task that represents the asynchronous save operation. The task result contains
		//     the number of state entries written to the database.
		//
		// 例外:
		//   T:Microsoft.EntityFrameworkCore.DbUpdateException:
		//     An error is encountered while saving to the database.
		//
		//   T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
		//     A concurrency violation is encountered while saving to the database. A concurrency
		//     violation occurs when an unexpected number of rows are affected during save.
		//     This is usually because the data in the database has been modified since it was
		//     loaded into memory.
		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
		//
		// 概要:
		//     Saves all changes made in this context to the database.
		//     This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
		//     to discover any changes to entity instances before saving to the underlying database.
		//     This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
		//     Multiple active operations on the same context instance are not supported. Use
		//     'await' to ensure that any asynchronous operations have completed before calling
		//     another method on this context.
		//
		// パラメーター:
		//   acceptAllChangesOnSuccess:
		//     Indicates whether Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges
		//     is called after the changes have been sent successfully to the database.
		//
		//   cancellationToken:
		//     A System.Threading.CancellationToken to observe while waiting for the task to
		//     complete.
		//
		// 戻り値:
		//     A task that represents the asynchronous save operation. The task result contains
		//     the number of state entries written to the database.
		//
		// 例外:
		//   T:Microsoft.EntityFrameworkCore.DbUpdateException:
		//     An error is encountered while saving to the database.
		//
		//   T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
		//     A concurrency violation is encountered while saving to the database. A concurrency
		//     violation occurs when an unexpected number of rows are affected during save.
		//     This is usually because the data in the database has been modified since it was
		//     loaded into memory.
		public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
		//
		// 概要:
		//     Creates a Microsoft.EntityFrameworkCore.DbSet`1 that can be used to query and
		//     save instances of TEntity.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of entity for which a set should be returned.
		//
		// 戻り値:
		//     A set for the given entity type.
		public DbSet<TEntity> Set<TEntity>([NotNull] string name) where TEntity : class;
		//
		// 概要:
		//     Creates a Microsoft.EntityFrameworkCore.DbSet`1 that can be used to query and
		//     save instances of TEntity.
		//
		// 型パラメーター:
		//   TEntity:
		//     The type of entity for which a set should be returned.
		//
		// 戻り値:
		//     A set for the given entity type.
		public DbSet<TEntity> Set<TEntity>() where TEntity : class;
		//
		// 概要:
		//     Returns a string that represents the current object.
		//
		// 戻り値:
		//     A string that represents the current object.
		public EntityEntry<TEntity> Update<TEntity>([NotNull] TEntity entity) where TEntity : class;
		//
		// 概要:
		//     Begins tracking the given entity and entries reachable from the given entity
		//     using the Microsoft.EntityFrameworkCore.EntityState.Modified state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Modified
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure new entities will be inserted, while existing entities
		//     will be updated. An entity is considered to have its primary key value set if
		//     the primary key property is set to anything other than the CLR default for the
		//     property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Modified.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entity:
		//     The entity to update.
		//
		// 戻り値:
		//     The Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry for the entity.
		//     The entry provides access to change tracking information and operations for the
		//     entity.
		public EntityEntry Update([NotNull] object entity);
		//
		// 概要:
		//     Begins tracking the given entities and entries reachable from the given entities
		//     using the Microsoft.EntityFrameworkCore.EntityState.Modified state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Modified
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure new entities will be inserted, while existing entities
		//     will be updated. An entity is considered to have its primary key value set if
		//     the primary key property is set to anything other than the CLR default for the
		//     property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Modified.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entities:
		//     The entities to update.
		public void UpdateRange([NotNull] IEnumerable<object> entities);
		//
		// 概要:
		//     Begins tracking the given entities and entries reachable from the given entities
		//     using the Microsoft.EntityFrameworkCore.EntityState.Modified state by default,
		//     but see below for cases when a different state will be used.
		//     Generally, no database interaction will be performed until Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		//     is called.
		//     A recursive search of the navigation properties will be performed to find reachable
		//     entities that are not already being tracked by the context. All entities found
		//     will be tracked by the context.
		//     For entity types with generated keys if an entity has its primary key value set
		//     then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Modified
		//     state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		//     state. This helps ensure new entities will be inserted, while existing entities
		//     will be updated. An entity is considered to have its primary key value set if
		//     the primary key property is set to anything other than the CLR default for the
		//     property type.
		//     For entity types without generated keys, the state set is always Microsoft.EntityFrameworkCore.EntityState.Modified.
		//     Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		//     state of only a single entity.
		//
		// パラメーター:
		//   entities:
		//     The entities to update.
		public void UpdateRange([NotNull] params object[] entities);
	}
}
