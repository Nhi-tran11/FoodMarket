     import React, { useState, useRef } from 'react';

import './InviteFriend.css';
const InviteFriend = () => {
  const [friendName, setFriendName] = useState('');
  const [friendEmail, setFriendEmail] = useState('');
  const [inviteSuccessMessage, setInviteSuccessMessage] = useState('');
  const inviteSectionRef = useRef(null);

  const handleInviteSubmit = (event) => {
    event.preventDefault();
    // Simulate sending invite (replace with actual API call)
    setTimeout(() => {
      setInviteSuccessMessage(`Invite sent to ${friendName} (${friendEmail})! They will receive a discount on their first order.`);
      // Clear form fields
      setFriendName('');
      setFriendEmail('');
    }, 1000);
  };

  return (
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
  );
};

export default InviteFriend;