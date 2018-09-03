using System;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;
using System.Linq;

namespace FoodIndustryService.Core.Tests.Steps
{
  [Binding]
  public class OrderSteps
  {
    [When(@"I press add order and set customer info")]
    public void WhenIPressAddOrderAndSetCustomerInfo(Table table)
    {
      OrderSystem orderSystem = new OrderSystem();
      OrderCart orderCard = ScenarioContext.Current.Get<OrderCart>("foodOrderCart");
      orderCard.CustomerName = table.Rows[0]["Name"];
      orderSystem.AddOrder(orderCard);

      ScenarioContext.Current.Set(orderSystem, "orderSystem");
    }


    [Then(@"All price calculated with price policy is")]
    public void ThenAllPriceCalculatedWithPricePolicyIs(Table table)
    {
      OrderCart foodOrderCart = ScenarioContext.Current.Get<OrderCart>("foodOrderCart");

      Assert.AreEqual(foodOrderCart.CalculatePrice(), decimal.Parse(table.Rows[0]["Price"]));
    }

    [Given(@"I have added following products into the order")]
    [Then(@"I have added following products into the order")]
    public void ThenIHaveAddedFollowingProductsIntoTheOrder(Table table)
    {
      OrderCart foodOrderCart;

      if (!ScenarioContext.Current.TryGetValue<OrderCart>("foodOrderCart", out foodOrderCart))
      {
        foodOrderCart = new OrderCart();
      }

      foodOrderCart.AddItems(table.CreateSet<FoodOrderItem>());

      ScenarioContext.Current.Set(foodOrderCart, "foodOrderCart");
    }

    [Then(@"I have deleted following products from the order")]
    public void ThenIHaveDeletedFollowingProductsFromTheOrder(Table table)
    {
      OrderCart foodOrderCart = ScenarioContext.Current.Get<OrderCart>("foodOrderCart");

      foreach (var row in table.Rows)
      {
        var item = foodOrderCart.Items.First(i => i.Name == row["Name"] && i.Price == decimal.Parse(row["Price"]));
        var amount = int.Parse(row["Amount"]);

        if (item.Amount == amount)
        {
          foodOrderCart.RemoveItem(item);
        }
        else if (item.Amount > amount)
        {
          item.Amount -= amount;
        }
        else
        {
          throw new NotImplementedException();
        }
      }

      ScenarioContext.Current.Set(foodOrderCart, "foodOrderCart");
    }


    [Then(@"the Order should be added on the Orders system")]
    public void ThenTheOrderShouldBeAddedOnTheOrdersSystem(Table table)
    {
      OrderSystem orderSystem = ScenarioContext.Current.Get<OrderSystem>("orderSystem");
      FoodOrder order = orderSystem.GetFoodOrder(table.Rows[0]["CustomerName"]);

      IEnumerable<FoodOrderItem> foodOrderItems = table.CreateSet<FoodOrderItem>();

      Assert.That(order.Items, Is.EquivalentTo(foodOrderItems));
    }
  }

  public class OrderCart
  {
    private readonly List<FoodOrderItem> _foodOrderItems;

    public IEnumerable<FoodOrderItem> Items
    {
      get { return _foodOrderItems; }
    }

    public string CustomerName { get; set; }

    public OrderCart()
    {
      _foodOrderItems = new List<FoodOrderItem>();
    }

    public void AddItem(FoodOrderItem foodOrderItem)
    {
      _foodOrderItems.Add(foodOrderItem);
    }

    public bool RemoveItem(FoodOrderItem foodOrderItem)
    {
      return _foodOrderItems.Remove(foodOrderItem);
    }

    public decimal CalculatePrice()
    {
      return _foodOrderItems.Sum(i => i.Amount * i.Price);
    }

    public void AddItems(IEnumerable<FoodOrderItem> foodOrderItems)
    {
      _foodOrderItems.AddRange(foodOrderItems);
    }
  }

  public class FoodOrder
  {
    private readonly string _customerName;
    private readonly IEnumerable<FoodOrderItem> _items;

    public IEnumerable<FoodOrderItem> Items
    {
      get { return _items; }
    }

    public string CustomerName
    {
      get { return _customerName; }
    }

    public FoodOrder(string customerName, IEnumerable<FoodOrderItem> items)
    {
      _customerName = customerName;
      _items = items;
    }
  }

  public class OrderSystem
  {
    private readonly List<FoodOrder> _orders;

    public OrderSystem()
    {
      _orders = new List<FoodOrder>();
    }

    public FoodOrder GetFoodOrder(string customerName)
    {
      return _orders.FirstOrDefault(o => o.CustomerName == customerName);
    }

    public void AddOrder(OrderCart orderCart)
    {
      _orders.Add(new FoodOrder(orderCart.CustomerName, orderCart.Items));
    }
  }

  public class FoodOrderItem
  {
    public override string ToString()
    {
      return $"{nameof(this.Name)}: {this.Name}, {nameof(this.Amount)}: {this.Amount}, {nameof(this.Price)}: {this.Price}";
    }

    protected bool Equals(FoodOrderItem other)
    {
      return string.Equals(this.Name, other.Name) && this.Amount == other.Amount && this.Price == other.Price;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((FoodOrderItem)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ this.Amount;
        hashCode = (hashCode * 397) ^ this.Price.GetHashCode();
        return hashCode;
      }
    }

    public string Name { get; set; }
    public int Amount { get; set; }
    public decimal Price { get; set; }
  }
}
