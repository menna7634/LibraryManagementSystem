

document.getElementById('profileForm').addEventListener('submit', function(event) {
    event.preventDefault(); 

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const membership = document.getElementById('membership').value;

   
    const userProfile = {
        name: name,
        email: email,
        membership: membership
    };

        localStorage.setItem('userProfile', JSON.stringify(userProfile));

  
    alert('Profile updated successfully!');

  
    loadUserProfile();
});


function loadUserProfile() {
    const savedProfile = JSON.parse(localStorage.getItem('userProfile'));
    if (savedProfile) {
        document.getElementById('name').value = savedProfile.name;
        document.getElementById('email').value = savedProfile.email;
        document.getElementById('membership').value = savedProfile.membership;
    }
}


window.onload = function() {
    loadUserProfile();
};
