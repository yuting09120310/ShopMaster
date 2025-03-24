// 購物車功能的JavaScript
$(document).ready(function() {
    // 增加數量按鈕
    $(document).on('click', '.quantity-btn[data-action="increase"]', function() {
        var input = $(this).siblings('.quantity-input');
        var currentValue = parseInt(input.val());
        var maxValue = parseInt(input.attr('max'));
        
        if (currentValue < maxValue) {
            input.val(currentValue + 1);
            updateCartItem($(this).closest('.cart-item'));
        }
    });
    
    // 減少數量按鈕
    $(document).on('click', '.quantity-btn[data-action="decrease"]', function() {
        var input = $(this).siblings('.quantity-input');
        var currentValue = parseInt(input.val());
        var minValue = parseInt(input.attr('min'));
        
        if (currentValue > minValue) {
            input.val(currentValue - 1);
            updateCartItem($(this).closest('.cart-item'));
        }
    });
    
    // 數量輸入框變更
    $(document).on('change', '.quantity-input', function() {
        updateCartItem($(this).closest('.cart-item'));
    });
    
    // 移除按鈕
    $(document).on('click', '.remove-btn', function() {
        var cartItem = $(this).closest('.cart-item');
        var cartId = cartItem.data('id');
        
        // 發送刪除請求
        $.ajax({
            url: '/FrontEnd/Cart/Delete',
            type: 'POST',
            data: { cartId: cartId },
            success: function(response) {
                if (response.success) {
                    // 移除項目並更新頁面
                    cartItem.fadeOut(300, function() {
                        $(this).remove();
                        
                        // 檢查是否還有商品
                        if ($('.cart-item').length === 0) {
                            location.reload(); // 重新載入頁面顯示空購物車
                        } else {
                            updateOrderSummary();
                        }
                    });
                    
                    // 更新購物車計數
                    updateCartCount();
                } else {
                    alert(response.message);
                }
            },
            error: function() {
                alert('刪除失敗，請稍後再試');
            }
        });
    });
    
    // 更新購物車項目
    function updateCartItem(cartItem) {
        var cartId = cartItem.data('id');
        var quantity = cartItem.find('.quantity-input').val();
        
        $.ajax({
            url: '/FrontEnd/Cart/Update',
            type: 'POST',
            data: { cartId: cartId, quantity: quantity },
            success: function(response) {
                if (response.success) {
                    updateOrderSummary();
                    updateCartCount();
                } else {
                    alert(response.message);
                }
            },
            error: function() {
                alert('更新失敗，請稍後再試');
            }
        });
    }
    
    // 更新訂單摘要
    function updateOrderSummary() {
        // 計算總金額
        var total = 0;
        $('.cart-item').each(function() {
            var price = parseFloat($(this).find('.text-primary').text().replace('$', ''));
            var quantity = parseInt($(this).find('.quantity-input').val());
            total += price * quantity;
        });
        
        // 更新顯示
        $('.card-body .fw-bold:last').text('$' + total.toFixed(0));
        $('.card-body .d-flex:first-child span:last-child').text('$' + total.toFixed(0));
    }
    
    // 更新購物車計數
    function updateCartCount() {
        $.ajax({
            url: '/FrontEnd/Cart/GetCartCount',
            type: 'GET',
            success: function(response) {
                $('#cartCount').text(response.count);
            }
        });
    }
});