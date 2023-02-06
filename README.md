# EFCore 持久化实体操作记录
##### 1、重写 OnModelCreating 配置持久化表
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    //配置默认实体指定存储的表名
    modelBuilder.UseChangesHistory("TableName");
    //配置自定义实体需继承AutoChangesHistory 指定存储的表名
    modelBuilder.UseChangesHistory<ChangesHistory>("TableName");
}
```

##### 2、重写 SaveChangesAsync 配置持久化表
```csharp
public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
{
    
    //启用默认实体记录
    this.AutoHistory();
    
    //启用自定义实体记录
    this.AutoHistory<ChangesHistory>(x =>
    {
        x.ModifyUser = "006";
        x.ModifyName = "老六";
    });
    
    return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
}
```

##### 3、配置需要记录的实体

```csharp
[History("user", EntityState.Modified | EntityState.Added | EntityState.Deleted)]
[Table("user")]
public class User
{
   public User()
   {
       Id = Guid.NewGuid().ToString();
   }

   [Key, Column("id")]
   public string Id { get; set; }

   [Column("name")]
   public string Name { get; set; }

   [Column("age")]
   public int Age { get; set; }
   
   //需要忽略变更的字段
   [IgnoreChanges]
   [Column("sex")]
   public string Sex { get; set; }
}
```
##### 4、获取差异
```csharp
var history = await templateDb.ChangesHistorys.FirstOrDefaultAsync(x => x.SourceKey == "1" && x.Source == "user" && x.Kind == EntityState.Modified);
var difference = history.GetDifferences(new Dictionary<string, string>()
{
    ["Name"] = "姓名",
    ["Age"] = "年龄",
    ["Sex"] = "性别"
});
```
