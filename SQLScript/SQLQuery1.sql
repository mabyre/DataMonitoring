SELECT Table1.Value1 as VALUE, Table1.Value2 as GROUP1
FROM Table1
WHERE DateCreation >= GETDATE()
GROUP BY Table1.Value2