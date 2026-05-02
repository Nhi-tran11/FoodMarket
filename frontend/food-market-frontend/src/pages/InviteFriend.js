import React, { useState, useRef } from 'react';
import { gql } from '@apollo/client';
import './InviteFriend.css';
import { useMutation } from '@apollo/client';
const InviteFriend = () => {
  const [friendName, setFriendName] = useState('');
  const [friendEmail, setFriendEmail] = useState('');
  const [inviteSuccessMessage, setInviteSuccessMessage] = useState('');
  const inviteSectionRef = useRef(null);
  const SEND_INVITATION =gql`
  mutation sendInvitationEmail($inviterId: Int!, $inviteeEmail: String!) {
    sendInvitationEmail(inviterId: $inviterId, inviteeEmail: $inviteeEmail) {
      success
      message
    }
  }
`;
  const user = JSON.parse(localStorage.getItem('user'));
  const inviterId = user ? user.id : null;
const [sendInvite] = useMutation(SEND_INVITATION);
  const handleInviteSubmit = (event) => {
    event.preventDefault();

    sendInvite({ variables: {inviterId, inviteeEmail: friendEmail}})
    .then(response => {
      const {success, message} = response.data.sendInvitationEmail;
      if (success) {
        setInviteSuccessMessage(message);
      }
    })
    .catch(error => {
      console.error('Error sending invitation:', error);
    });
    setTimeout(() => {
      setInviteSuccessMessage(inviteSuccessMessage);
      // Clear form fields
      setFriendName('');
      setFriendEmail('');
      
    }, 1000);
  };

  return (
    inviterId ? (

    <div className="invite-friend-page">
      <h2 className="page-title">Invite a Friend and Get Discounts!</h2>
      <p className="page-description">
        Share the love of delicious food with your friends! Invite them to join our food market and both of you will receive exclusive discounts on your next orders.
      </p>          
     <section className="invite-friend-section" ref={inviteSectionRef}>
        <div className="invite-friend-card">
          <h3 className="invite-title">Invite a Friend</h3>
          <p className="invite-description">
            Enter your friend&apos;s details and send them an invite to enjoy special discounts.
          </p>

          <form className="invite-form" onSubmit={handleInviteSubmit}>
            <div className="invite-form-group">
              <label htmlFor="friendName">Friend Name</label>
              <input
                id="friendName"
                type="text"
                value={friendName}
                onChange={(event) => setFriendName(event.target.value)}
                placeholder="Enter friend name"
                required
              />
            </div>

            <div className="invite-form-group">
              <label htmlFor="friendEmail">Friend Email</label>
              <input
                id="friendEmail"
                type="email"
                value={friendEmail}
                onChange={(event) => setFriendEmail(event.target.value)}
                placeholder="Enter friend email"
                required
              />
            </div>

            <button type="submit" className="invite-submit-btn">Send Invite</button>
          </form>

          {inviteSuccessMessage && <p className="invite-success">{inviteSuccessMessage}</p>}
        </div>
      </section>
    </div>
  ) : (
    <div className="invite-friend-page">
      <h2>Please log in to invite friends</h2>
    </div>)
  );
};

export default InviteFriend;