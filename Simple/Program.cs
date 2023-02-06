// See https://aka.ms/new-console-template for more information
using EntityFrameworkCore.ChangesHistory;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var templateDb = new TemplateDbContext();
//templateDb.Users.Add(new User()
//{
//    Name = "a",
//    Age = 10,
//    Sex = "男"
//});

//templateDb.Users.Add(new User()
//{
//    Name = "c",
//    Age = 11,
//    Sex = "女"
//});

await templateDb.SaveChangesAsync();

var users = await templateDb.Users.Where(x => x.Name == "a").ToListAsync();
users.ForEach(user =>
{
    user.Age++;
});

await templateDb.SaveChangesAsync();

var history = await templateDb.ChangesHistorys
    .Where(x => x.SourceKey == users.First().Id && x.Source == "user" && x.Kind == EntityState.Modified)
    .OrderByDescending(x => x.ModifyTime)
    .FirstOrDefaultAsync();

var difference = history?.GetDifferences(new Dictionary<string, string>()
{
    ["Name"] = "姓名",
    ["Age"] = "年龄",
    ["Sex"] = "性别"
});
