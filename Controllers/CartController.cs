using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarketplace.Data;
using OnlineMarketplace.Data.Services;
using OnlineMarketplace.Models;
using OnlineMarketplace.Models.ViewModels;
using System.Security.Claims;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;

    public CartController(ICartService cartService, IOrderService orderService, IProductService productService, IPaymentService paymentService)
    {
        _cartService = cartService;
        _orderService = orderService;
        _productService = productService;
        _paymentService = paymentService;
    }

    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(HttpContext);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        await _cartService.AddToCartAsync(productId, HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        await _cartService.RemoveFromCartAsync(productId, HttpContext);
        return RedirectToAction(nameof(Index));
    }
	
	[HttpPost]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        await _cartService.UpdateQuantityAsync(productId, quantity, HttpContext);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = UserRoles.User)]
    public async Task<IActionResult> Checkout()
    {
        var cart = await _cartService.GetCartAsync(HttpContext);
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction(nameof(Index));
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            TempData["Error"] = "Please log in to complete the purchase.";
            return RedirectToAction("Login", "Account");
        }

        var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
                            .Cast<PaymentMethod>()
                            .ToList();
        ViewData["PaymentMethodList"] = new SelectList(paymentMethods);
        return View();
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.User)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(OrderVM orderVM)
    {
        var cart = await _cartService.GetCartAsync(HttpContext);
        if (cart == null || !cart.Items.Any())
        {
            TempData["Error"] = "Your cart is empty!";
            return RedirectToAction(nameof(Index));
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            TempData["Error"] = "Please log in to complete the purchase.";
            return RedirectToAction("Login", "Account");
        }

        var productIds = cart.Items.Select(i => i.ProductId).ToList();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                if (product.UnitsInStock < item.Quantity)
                {
                    TempData["Error"] = $"{product.Name} is out of stock! There are only {product.UnitsInStock} items.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
                product.UnitsInStock -= item.Quantity;
        }

        // Create the order
        var orderAddress = new OrderAddress
        {
            City = orderVM.City,
            District = orderVM.District,
            Street = orderVM.Street,
            ZibCode = orderVM.ZipCode
        };
        var order = new Order
        {
            UserId = userId,
            OrderPlaced = DateTime.Now,
            Status = OrderStatus.Pending,
            Address = orderAddress
        };
        
        // Save the order and its details
        await _orderService.AddAsync(order);
        await _orderService.SaveAsync();

        var orderDetails = cart.Items.Select(item => new OrderDetail
        {
            ProductId = item.ProductId,
            OrderId = order.Id,
            Quantity = item.Quantity,
            Price = item.Price * item.Quantity
        }).ToList();
        await _orderService.AddOrderDetailsAsync(orderDetails);

        // Prepare for payment
        var paymentVM = new PaymentViewModel
        {
            OrderId = order.Id,
            Amount = orderDetails.Sum(od => od.Price),
            Method = orderVM.PaymentMethod
        };

        bool isSuccess = await _paymentService.ProcessPaymentAsync(userId, order, paymentVM);
        if (!isSuccess)
        {
            TempData["PaymentError"] = "Payment failed!";
            return RedirectToAction(nameof(Index));
        }

        // Clear the cart after successful checkout
        _cartService.ClearCart(HttpContext);

        // Redirect to order confirmation
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }

    [HttpPost]
    public IActionResult ClearCart()
    {
        _cartService.ClearCart(HttpContext);
        return RedirectToAction(nameof(Index));
    }
}
