import React, { Component } from 'react';

export class Home extends Component<unknown, unknown> {
  static displayName = Home.name;

  greetUser(user: string) {
    console.log(`Hi there, ${user}`);
  }
  render() {
    return (
      <div>
        <h5>What happens anywhere, stays there!</h5>
        <p>Kommer...</p>
      </div>
    );
  }
}
