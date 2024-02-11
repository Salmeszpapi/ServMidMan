function validateEmail(input) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const emailInput = input.value.trim();

    if (emailRegex.test(emailInput)) {
        // Valid email format
        input.setCustomValidity('');
        return true;
    } else {
        // Invalid email format
        input.setCustomValidity('Please enter a valid email address.');
        return false;
    }
    var emailElement = document.getElementById('email');
    emailElement.innerHTML = "feri";
}
