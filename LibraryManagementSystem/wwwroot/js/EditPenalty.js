document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('penaltyType').addEventListener('change', function () {
        const selectedType = this.value;

        const penaltyAmounts = {
            "DamagedBook": 150.00,
            "NonReturn": 200.00,
            "UnpaidPreviousPenalties": 10.00,
            "ViolationOfLibraryPolicies": 20.00
        };

        const amount = penaltyAmounts[selectedType] || 0;
        document.getElementById('penaltyAmount').value = amount.toFixed(2);
    });
});