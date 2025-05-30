CREATE VIEW View_OrderSummary
AS
	SELECT o.Id, o.[OrderDate], COUNT(p.Id) AS Count, o.TotalValue
	FROM [Orders] AS o
	JOIN [OrderProducts] AS p ON o.Id = p.OrderId
	GROUP BY o.Id, o.[OrderDate], o.TotalValue