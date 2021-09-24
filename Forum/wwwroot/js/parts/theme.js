class Theme extends Component {
	init() {
		const curUrl = window.location.pathname;
		console.log( curUrl );

		this.panelActive();
		this.checkInput();
	}

	panelActive() {
		const signUpButton = document.getElementById( 'signUp' );
		const signInButton = document.getElementById( 'signIn' );
		const container = document.getElementById( 'container' );

		signUpButton.addEventListener( 'click', () => {
			container.classList.add( 'right-panel-active' );
		});

		signInButton.addEventListener( 'click', () => {
			container.classList.remove( 'right-panel-active' );
		});
	}

	checkInput() {
		const input = document.querySelectorAll( '.input-field' );

		input.forEach( function( element ) {
			console.log(element);
		})
	}
}

Theme.addComponent( 'body' );
